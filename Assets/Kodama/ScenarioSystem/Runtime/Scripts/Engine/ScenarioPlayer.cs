using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPlayer : IScenarioPlayerForCommand, IDisposable {
        private class PlayProcess {
            public Scenario Scenario {get;set;}
            public int CurrentPageIndex {get;set;}
            public ScenarioPage CurrentPage => Scenario.Pages[CurrentPageIndex];
            public int CurrentCommandIndex {get;set;}
            public CommandBase CurrentCommand => CurrentPage.Commands[CurrentCommandIndex];
            public List<VariableBase> Variables = new List<VariableBase>();

            public PlayProcess(Scenario scenario) {
                Scenario = scenario;
                CurrentPageIndex = 0;
                CurrentCommandIndex = 0;
                Variables = scenario?.Variables.Select(x => x.Copy()).ToList();
            }
        }

        private readonly ScenarioCache _scenarioCache;
        private readonly ServiceLocator _serviceLocator;
        private readonly ScenarioPreloadManager _scenarioPreloadManager;

        private PlayProcess _currentProcess;
        // 記述簡略化のためのプロパティ
        private Scenario CurrentScenario {
            get => _currentProcess?.Scenario;
            set => _currentProcess.Scenario = value;
        }
        private int CurrentPageIndex {
            get => _currentProcess.CurrentPageIndex;
            set => _currentProcess.CurrentPageIndex = value;
        }
        private ScenarioPage CurrentPage => _currentProcess.CurrentPage;
        private int CurrentCommandIndex {
            get => _currentProcess.CurrentCommandIndex;
            set => _currentProcess.CurrentCommandIndex = value;
        }
        private CommandBase CurrentCommand => _currentProcess.CurrentCommand;
        private List<VariableBase> CurrentVariables {
            get => _currentProcess.Variables;
            set => _currentProcess.Variables = value;
        }

        // 再開するためのプロセスを積んでおく
        private Stack<PlayProcess> _processStack = new Stack<PlayProcess>();

        public bool WaitingPreload {get; private set;}

        public bool IsPlaying {get; private set;}

        public bool IsPaused {get; private set;}
        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        private bool _skipCommandIndexIncrement = false;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ScenarioPlayer(
            ScenarioCache scenarioCache,
            ServiceLocator serviceLocator,
            ScenarioPreloadManager scenarioPreloadManager
        ) {
            _scenarioCache = scenarioCache;
            _serviceLocator = serviceLocator;
            _scenarioPreloadManager = scenarioPreloadManager;
        }

        public void Dispose() {
            _cts.Cancel();
        }

        public void PlayScenario(string scenarioName, CancellationToken cancellationToken = default) {
            PlayScenarioAsync(scenarioName, cancellationToken).Forget();
        }

        public async UniTask PlayScenarioAsync(string scenarioName, CancellationToken cancellationToken = default) {
            // 排他制御
            if(IsPlaying) return;

            Scenario scenario = _scenarioCache.FindScenarioByName(scenarioName);
            if(scenario == null) {
                Debug.Log($"ScenarioEngine doesn't have scenario\"{scenarioName}\"");
                return;
            }
            _currentProcess = new PlayProcess(scenario);
            await PlayAsyncInternal(cancellationToken);
        }

        private async UniTask PlayAsyncInternal(CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _cts.Token)
                .Token;

            IsPlaying = true;

            CommandService commandService = new CommandService(this, _serviceLocator);

            try {
                while(IsPlaying) {
                    // プリロード中なら待機
                    if(_scenarioPreloadManager.IsPreloading(CurrentScenario)) {
                        WaitingPreload = true;
                        await UniTask.WaitUntil(
                            () => _scenarioPreloadManager.IsPreloading(CurrentScenario) == false,
                            cancellationToken: linkedToken
                        );
                        WaitingPreload = false;
                    }

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
                    // コマンドインデックスが最大になったら終了
                    if(CurrentCommandIndex >= CurrentPage.Commands.Count) {
                        // 現在のプロセスを終了
                        TerminateCurrentProcess();
                        // スタックにプロセスが積まれていたら再開
                        if(_processStack.Any()) {
                            _currentProcess = _processStack.Pop();
                            CurrentCommandIndex++;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            catch(OperationCanceledException) {}
            catch(Exception e) {
                Debug.LogError($"Scenario = \"{CurrentScenario.name}\", PageIndex = {CurrentPageIndex}, CommandIndex = {CurrentCommandIndex}, CommandType = {CurrentCommand.GetType()}\n{e}");
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
                    // 例外をログ出力
                    ((Action)(async () => {
                        // ログ出力用に値を保持
                        Scenario scenario = CurrentScenario;
                        int pageIndex = CurrentPageIndex;
                        int commandIndex = CurrentCommandIndex;
                        CommandBase commandBase = CurrentCommand;

                        try {
                            await asyncCommand.ExecuteAsync(commandService, cancellationToken);
                        }
                        catch(OperationCanceledException) {}
                        catch(Exception e) {
                            Debug.LogError($"Scenario = \"{scenario.name}\", PageIndex = {pageIndex}, CommandIndex = {commandIndex}, CommandType = {commandBase.GetType()}\n{e}");
                            throw;
                        }
                    }))();
                }
            }
            else {
                command.Execute(commandService);
            }
        }

        private void StackCurrentProcess() {
            _processStack.Push(_currentProcess);
            _currentProcess = null;
        }

        private void TerminateCurrentProcess() {
            // スタックに再開用の情報が積まれていたら処理は行わない
            if(_processStack.FirstOrDefault(x => x.Scenario == CurrentScenario) == null) {
                if(_scenarioCache.Find(CurrentScenario).RemoveOnExitScenario) {
                    _scenarioCache.Remove(CurrentScenario);
                }
            }
            _currentProcess = null;
        }

        public void JumpToLabel(string targetLabel) {
            // まず現在のページを調べる
            for(int i = 0; i < CurrentPage.Commands.Count; i++) {
                if(CurrentPage.Commands[i] is LabelCommand labelCommand && labelCommand.Label == targetLabel) {
                    CurrentCommandIndex = i;
                    return;
                }
            }
            // 現在のシナリオを調べる
            for(int i = 0; i < CurrentScenario.Pages.Count; i++) {
                for(int j = 0; j < CurrentScenario.Pages[i].Commands.Count; j++) {
                    if(CurrentScenario.Pages[i].Commands[j] is LabelCommand labelCommand && labelCommand.Label == targetLabel) {
                        CurrentPageIndex = i;
                        CurrentCommandIndex = j;
                        return;
                    }
                }
            }
            Debug.LogWarning($"{nameof(JumpToLabel)} : target label \"{targetLabel}\" not found.");
        }

        public void JumpToPage(string pageName, bool returnOnExit = false) {
            ScenarioPage page = CurrentScenario.Pages.FirstOrDefault(x => x.Name == pageName);
            for(int i = 0; i < CurrentScenario.Pages.Count; i++) {
                if(CurrentScenario.Pages[i].Name == pageName) {
                    // 再開用の情報を積む
                    if(returnOnExit) {
                        PlayProcess newProcess = new PlayProcess(CurrentScenario) {
                            CurrentPageIndex = i
                        };
                        StackCurrentProcess();
                        _currentProcess = newProcess;
                    }
                    else {
                        CurrentPageIndex = i;
                        CurrentCommandIndex = 0;
                    }
                    _skipCommandIndexIncrement = true;
                    return;
                }
            }
            Debug.LogWarning($"{nameof(JumpToPage)} : target page not found.");
        }

        public void JumpToScenario(string scenarioName, bool returnOnExit = false) {
            Scenario newScenario = _scenarioCache.Scenarios.FirstOrDefault(x => x.name == scenarioName);
            if(newScenario == null) {
                Debug.LogWarning($"{nameof(JumpToScenario)} : target scenario \"{scenarioName}\" not found.");
                return;
            }
            // 再開用の情報を積む
            if(returnOnExit) {
                StackCurrentProcess();
            } else {
                TerminateCurrentProcess();
            }
            // 新シナリオ設定
            _currentProcess = new PlayProcess(newScenario);
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
#endregion
    }
}