using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace Kodama.ScenarioSystem {
    internal class PlayProcess : IPlayProcess {
        private readonly ServiceLocator _serviceLocator;

        public Stack<ScenarioPlayStatus> ScenarioPlayStatusStack {get;} = new Stack<ScenarioPlayStatus>();
        public ScenarioPlayStatus CurrentScenarioPlayStatus => ScenarioPlayStatusStack.Count > 0 ? ScenarioPlayStatusStack.Peek() : null;

        public bool IsPlaying {get;set;}

        public bool IsPaused {get; private set;}
        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        public bool WaitingPreload {get; private set;}

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();


        // 記述簡略化のためのプロパティ
        private AvailableScenario CurrentAvailable => CurrentScenarioPlayStatus.Available;
        private PagePlayStatus CurrentPagePlayStatus => CurrentScenarioPlayStatus.CurrentPagePlayStatus;
        private Scenario CurrentScenario => CurrentScenarioPlayStatus?.Scenario;
        private ScenarioPage CurrentPage => CurrentScenarioPlayStatus.CurrentPagePlayStatus.CurrentPage;
        private int CurrentCommandIndex {
            get => CurrentScenarioPlayStatus.CurrentPagePlayStatus.CurrentCommandIndex;
            set => CurrentScenarioPlayStatus.CurrentPagePlayStatus.CurrentCommandIndex = value;
        }
        private CommandBase CurrentCommand => CurrentScenarioPlayStatus.CurrentPagePlayStatus.CurrentCommand;
        private List<VariableBase> CurrentVariables => CurrentScenarioPlayStatus.Variables;
        private Block CurrentBlock => CurrentScenarioPlayStatus.CurrentPagePlayStatus.BlockStack.Peek();

        private bool _skipCommandIndexIncrement = false;

        public PlayProcess(ServiceLocator serviceLocator, AvailableScenario entry) {
            _serviceLocator = serviceLocator;
            ScenarioPlayStatusStack.Push(new ScenarioPlayStatus(entry));
        }

        public bool HasScenarioProcess => ScenarioPlayStatusStack.Count > 0;

        public void PushNewScenarioPlayStatus(AvailableScenario available) {
            ScenarioPlayStatusStack.Push(new ScenarioPlayStatus(available));
        }

        public ScenarioPlayStatus PopScenarioPlayStatus() {
            return ScenarioPlayStatusStack.Pop();
        }

        internal async UniTask PlayAsync(CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _cts.Token)
                .Token;

            CommandService commandService = new CommandService(this, _serviceLocator);

            IsPlaying = true;

            try {
                while(IsPlaying) {
                    // プリロード中なら待機
                    WaitingPreload = true;
                    await UniTask.WaitUntil(
                        () => CurrentAvailable.PreloadState == PreloadState.Completed,
                        cancellationToken: linkedToken
                    );
                    WaitingPreload = false;

                    // コマンド実行
                    await ExecuteCommandAsync(CurrentCommand, commandService,  linkedToken);

                    // ポーズ中なら解除まで待機
                    await UniTask.WaitUntil(() => IsPaused == false, cancellationToken: linkedToken);

                    // コマンドインデックス加算
                    if(_skipCommandIndexIncrement) {
                        _skipCommandIndexIncrement = false;
                    }
                    else {
                        CurrentCommandIndex++;
                    }

                    // ブロックの範囲を越えていたらブロックを破棄
                    while(true) {
                        Block block = PeekBlock();
                        if(block == null) break;
                        if(block.StartIndex <= CurrentCommandIndex && CurrentCommandIndex <= block.EndIndex) break;
                        PopBlock();
                    }

                    // コマンドインデックスが最大になったら終了
                    if(CurrentCommandIndex >= CurrentPage.Commands.Count) {
                        // ページプロセスをポップ
                        CurrentScenarioPlayStatus.PopPageProcess();
                        // ページプロセスが残っていたら再開
                        if(CurrentScenarioPlayStatus.HasPageProcess) {
                            CurrentCommandIndex++;
                        }
                        // ページプロセスが無くなったらシナリオプロセス終了
                        else {
                            // 現在のシナリオを終了
                            DisposeCurrentScenarioPlayStatus();
                            // スタックにシナリオ再生ステータスが積まれてなければ終了
                            if(HasScenarioProcess) {
                                CurrentCommandIndex++;
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
            }
            catch(OperationCanceledException) {}
            catch(Exception e) {
                LogCommandException(CurrentScenario.name, CurrentPage.Name, CurrentCommandIndex, CurrentCommand.GetType(), e);
                throw;
            }
            finally {
                IsPlaying = false;
            }
        }

        private async UniTask ExecuteCommandAsync(CommandBase command, CommandService commandService, CancellationToken cancellationToken) {
            if(command is AsyncCommandBase asyncCommand) {
                if (asyncCommand.Wait) {
                    await asyncCommand.ExecuteAsync(commandService, cancellationToken);
                }
                // 待機しない場合
                else {
                    ((Action)(async () => {
                        // ログ出力用に値を保持
                        Scenario scenario = CurrentScenario;
                        ScenarioPage page = CurrentPage;
                        int commandIndex = CurrentCommandIndex;
                        CommandBase commandBase = CurrentCommand;

                        try {
                            await asyncCommand.ExecuteAsync(commandService, cancellationToken);
                        }
                        catch(OperationCanceledException) {}
                        catch(Exception e) {
                            LogCommandException(scenario.name, page.Name, commandIndex, commandBase.GetType(), e);
                            throw;
                        }
                    }))();
                }
            }
            else {
                command.Execute(commandService);
            }
        }

        private void DisposeCurrentScenarioPlayStatus() {
            ScenarioPlayStatus poped = PopScenarioPlayStatus();
            AvailableScenario available = poped.Available;
            // スタックに再開用の情報が積まれていなければ、終了時処理を行う
            if(ScenarioPlayStatusStack.FirstOrDefault(x => x.Available == available) == null) {
                if(available.RemoveOnExitScenario) {
                    ScenarioManager.Instance.Remove(available);
                }
            }
        }

        public void JumpToIndex(int index) {
            if(index < 0 || CurrentPage.Commands.Count <= index) {
                Debug.LogWarning($"{nameof(JumpToIndex)} : index out of range. index must be 0 ～ {CurrentPage.Commands.Count}. but input is {index}.");
                return;
            }

            CurrentCommandIndex = index;
            _skipCommandIndexIncrement = true;
        }

        public void JumpToLabel(string targetLabel) {
            // まず現在のページを調べる
            for(int i = 0; i < CurrentPage.Commands.Count; i++) {
                if(CurrentPage.Commands[i] is LabelCommand labelCommand && labelCommand.Label == targetLabel) {
                    CurrentCommandIndex = i;
                    _skipCommandIndexIncrement = true;
                    return;
                }
            }
            Debug.LogWarning($"{nameof(JumpToLabel)} : target label \"{targetLabel}\" not found.");
        }

        public void JumpToBlockEnd(IBlockStart blockStart) {
            JumpToIndex(CurrentPage.FindBlockEndIndex(blockStart));
        }

        public void JumpToPage(string pageName, bool returnOnExit = false) {
            ScenarioPage page = CurrentScenario.Pages.FirstOrDefault(x => x.Name == pageName);
            if(page ==null) {
                Debug.LogWarning($"{nameof(JumpToPage)} : target page not found.");
                return;
            }

            // 再開用の情報を積む
            if(returnOnExit) {
                CurrentScenarioPlayStatus.PushNewPageProcess(page);
            }
            else {
                CurrentScenarioPlayStatus.SwitchToNewPageProcess(page);
            }
            _skipCommandIndexIncrement = true;
            return;
        }

        public void JumpToScenario(string scenarioName, bool returnOnExit = false) {
            AvailableScenario newAvailable = ScenarioManager.Instance.FindAvailableByName(scenarioName);
            if(newAvailable == null) {
                Debug.LogWarning($"{nameof(JumpToScenario)} : target scenario \"{scenarioName}\" not found.");
                return;
            }
            // 再開用の情報を積む
            if(returnOnExit) {
                PushNewScenarioPlayStatus(newAvailable);
            } else {
                DisposeCurrentScenarioPlayStatus();
                PushNewScenarioPlayStatus(newAvailable);
            }
            _skipCommandIndexIncrement = true;
        }

#region Variable
        public T GetVariableValue<T>(string variableName) {
            var castedVariables = CurrentVariables.OfType<Variable<T>>();
            if(castedVariables.Count() == 0) {
                Debug.LogError($"{typeof(T).Name} variable not found.");
            }
            var variable = castedVariables.Where(x => x.Name == variableName).FirstOrDefault();
            if(variable == null) {
                Debug.LogError($"variable (name = {variableName}) not found.");
            }
            return variable.Value;
        }
        public object GetVariableValue(Type variableType, string variableName) {
            var variables = CurrentVariables.Where(x => x.TargetType == variableType);
            if(variables.Count() == 0) {
                Debug.LogError($"{variableType.Name} variable not found.");
            }
            var variable = variables.Where(x => x.Name == variableName).FirstOrDefault();
            if(variable == null) {
                Debug.LogError($"variable (name = {variableName}) not found.");
            }
            return variable.GetValueAsObject();
        }

        public void SetVariableValue<T>(string variableName, T value) {
            var castedVariables = CurrentVariables.OfType<Variable<T>>();
            if(castedVariables.Count() == 0) {
                Debug.LogError($"{typeof(T).Name} variable not found.");
            }
            var variable = castedVariables.Where(x => x.Name == variableName).FirstOrDefault();
            if(variable == null) {
                Debug.LogError($"variable (name = {variableName}) not found.");
            }
            variable.Value = value;
        }
        public void SetVariableValue(Type variableType, string variableName, object value) {
            var variables = CurrentVariables.Where(x => x.TargetType == variableType);
            if(variables.Count() == 0) {
                Debug.LogError($"{variableType.Name} variable not found.");
            }
            var variable = variables.Where(x => x.Name == variableName).FirstOrDefault();
            if(variable == null) {
                Debug.LogError($"variable (name = {variableName}) not found.");
            }
            variable.SetValueAsObject(value);
        }
#endregion

#region Block
        public void SetUpAndPushBlock(IBlockStart blockStart, Block block) {
            block.StartIndex = CurrentPage.IndexOf(blockStart as CommandBase);
            block.EndIndex = CurrentPage.FindBlockEndIndex(blockStart);
            CurrentPagePlayStatus.BlockStack.Push(block);
        }

        public Block PopBlock() => CurrentPagePlayStatus.BlockStack.Count > 0 ? CurrentPagePlayStatus.BlockStack.Pop() : null;

        public Block PeekBlock() => CurrentPagePlayStatus.BlockStack.Count > 0 ? CurrentPagePlayStatus.BlockStack.Peek() : null;
#endregion

        private void LogCommandException(string scenarioName, string pageName, int commandIndex, Type commandType, Exception e) {
            Debug.LogError($"Scenario = \"{scenarioName}\", PageName = {pageName}, CommandIndex = {commandIndex}, CommandType = {commandType.Name}\n{e}");
        }
    }
}