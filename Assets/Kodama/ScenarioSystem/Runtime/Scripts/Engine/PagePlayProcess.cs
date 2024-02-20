using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class PagePlayProcess : IPagePlayProcess {
        private readonly ScenarioPlayProcess _scenarioProcess; 
        public ScenarioPlayProcess ScenarioProcess => _scenarioProcess;

        private readonly ScenarioPage _page;
        private int _currentCommandIndex;
        private readonly Stack<Block>_blockStack = new Stack<Block>();

        private bool _skipCommandIndexIncrement = false;

        private bool _isPaused;
        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;

        private int _asyncExecutingCommandCounter;
        private Action<PagePlayProcess> _onAllCommandFinished;

        // 後続処理情報
        public ScenarioPage SubsequentPage {get; set;}
        public Scenario SubsequentScenario {get; set;}
        public IReadOnlyList<ICallArg> SubsequentScenarioCallArgs {get; set;}
        public bool SwitchRootProcessOnPlaySubsequentScenario {get; set;}
        public Action OnNewRootProcessFinished {get; set;}

        public PagePlayProcess(ScenarioPlayProcess scenarioProcess, ScenarioPage page, Action<PagePlayProcess> onAllCommandFinished) {
            _scenarioProcess = scenarioProcess;
            _page = page;
            _onAllCommandFinished = onAllCommandFinished;
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken = default) {
            try {
                cancellationToken.ThrowIfCancellationRequested();

                // コマンドに提供する機能群を作成
                CommandService commandService = new CommandService(this, _scenarioProcess.RootProcess.ServiceLocator);

                while(true) {
                    CommandBase command = _page.Commands[_currentCommandIndex];

                    // コマンド実行
                    try {
                        await ExecuteCommandAsync(command, commandService,  cancellationToken);
                    }
                    catch(OperationCanceledException) {
                        throw;
                    }
                    catch(Exception e) {
                        Debug.Log($"<b>Exception</b>[ " + e.GetType().FullName + " ] was thrown in " + CreateLogExceptionHeader(false, command));
                        throw;
                    }

                    // ポーズ中なら解除まで待機
                    await UniTask.WaitUntil(() => _isPaused == false, cancellationToken: cancellationToken);

                    // コマンドインデックス加算
                    if(_skipCommandIndexIncrement) {
                        _skipCommandIndexIncrement = false;
                    }
                    else {
                        _currentCommandIndex++;
                    }

                    // ブロックの範囲を越えていたらブロックを破棄
                    while(true) {
                        Block block = PeekBlock();
                        if(block == null) break;
                        if(block.StartIndex <= _currentCommandIndex && _currentCommandIndex <= block.EndIndex) break;
                        PopBlock();
                    }

                    // コマンドインデックスが最大になったら終了
                    if(_currentCommandIndex >= _page.Commands.Count) {
                        break;
                    }
                }
            }
            finally {
                // 非同期実行中コマンドの終了を待って発火。
                // 後続処理は即座に実行できるように処理は返す。
                UniTask.Void(async () => {
                    try {
                        await UniTask.WaitUntil(() => _asyncExecutingCommandCounter == 0, cancellationToken: cancellationToken);
                    }
                    finally {
                        _onAllCommandFinished.Invoke(this);
                    }
                });
            }
        }

        private async UniTask ExecuteCommandAsync(CommandBase command, CommandService commandService, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(command is AsyncCommandBase asyncCommand) {
                if(asyncCommand.AsyncCommandSetting.Wait) {
                    await asyncCommand.ExecuteAsync(commandService, cancellationToken);
                }
                // 待機しない場合
                else {
                    UniTask awaiter = UniTask.Create(async () => {
                        _asyncExecutingCommandCounter++;
                        try {
                            await asyncCommand.ExecuteAsync(commandService, cancellationToken);
                        }
                        finally {
                            _asyncExecutingCommandCounter--;
                        }
                    });
                    if(asyncCommand.AsyncCommandSetting.SetUniTaskTo.IsEmpty()) {
                        awaiter.ForgetAndLogException(CreateLogExceptionHeader(true, asyncCommand));
                    }
                    else {
                        FindVariable<UniTask>(asyncCommand.AsyncCommandSetting.SetUniTaskTo.Id).Value = awaiter;
                    }
                }
            }
            else {
                command.Execute(commandService);
            }
        }

        public void JumpToEndIndex() {
            JumpToIndex(_page.Commands.Count);
        }

        public void JumpToIndex(int index) {
            if(index < 0 || _page.Commands.Count < index) {
                Debug.LogWarning($"{nameof(JumpToIndex)} : index out of range. index must be 0 ～ {_page.Commands.Count}. but input is {index}.");
                return;
            }

            _currentCommandIndex = index;
            _skipCommandIndexIncrement = true;
        }

        public void JumpToLabel(string targetLabel) {
            for(int i = 0; i < _page.Commands.Count; i++) {
                if(_page.Commands[i] is LabelCommand labelCommand && labelCommand.Label == targetLabel) {
                    _currentCommandIndex = i;
                    _skipCommandIndexIncrement = true;
                    return;
                }
            }
            Debug.LogWarning($"{nameof(JumpToLabel)} : target label \"{targetLabel}\" not found.");
        }

        public void JumpToBlockEnd(IBlockStart blockStart) {
            JumpToIndex(_page.FindBlockEndIndex(blockStart));
        }

#region Block
        public void SetUpAndPushBlock(IBlockStart blockStart, Block block) {
            block.StartIndex = _page.IndexOf(blockStart as CommandBase);
            block.EndIndex = _page.FindBlockEndIndex(blockStart);
            _blockStack.Push(block);
        }

        public Block PopBlock() => _blockStack.Count > 0 ? _blockStack.Pop() : null;

        public Block PeekBlock() => _blockStack.Count > 0 ? _blockStack.Peek() : null;
#endregion

#region Variable
        public Variable<T> FindVariable<T>(IVariableKey<T> variableKey) {
            return FindVariable<T>(variableKey.Id);
        }

        public Variable<T> FindVariable<T>(string id) {
            var targetTypeVariables = _scenarioProcess.Variables.OfType<Variable<T>>();
            if(targetTypeVariables.Count() == 0) {
                Debug.LogError($"{typeof(T).Name} variable doesn't exist.");
            }
            var targetVariable = targetTypeVariables.FirstOrDefault(x => x.Id == id);
            if(targetVariable == null) {
                Debug.LogError($"variable not found.");
            }
            return targetVariable;
        }

        public VariableBase FindVariable(IVariableKey variableKey) {
            return FindVariable(variableKey.TargetType, variableKey.Id);
        }

        public VariableBase FindVariable(Type targetType, string id) {
            var targetTypeVariables = _scenarioProcess.Variables.Where(x => x.TargetType == targetType);
            if(targetTypeVariables.Count() == 0) {
                Debug.LogError($"{targetType.Name} variable doesn't exist.");
            }
            var targetVariable = targetTypeVariables.FirstOrDefault(x => x.Id == id);
            if(targetVariable == null) {
                Debug.LogError($"variable not found.");
            }
            return targetVariable;
        }
#endregion

        private string CreateLogExceptionHeader(bool isAsync, CommandBase command) {
            return $"{(isAsync ? "[Async] " : "")} {command.LogHeader}";
        }
    }
}