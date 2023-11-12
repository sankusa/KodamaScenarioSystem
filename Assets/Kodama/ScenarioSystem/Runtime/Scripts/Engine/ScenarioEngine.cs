using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading;
using System.Threading.Tasks;
using System;


#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

namespace Kodama.ScenarioSystem {
    public class ScenarioEngine : MonoBehaviour, IScenarioEngine, IScenarioEngineForInternal {
        [SerializeField] private List<Scenario> _scenarios;
        private ScenarioCache _scenarioCache;

        [SerializeField] private bool _playFirstScenarioOnAwake;
        /// <summary>
        /// コンポーネントの参照登録
        /// </summary>
        [SerializeField] private List<Component> _componentBindings;
        
        private Scenario _currentScenario;
        private int _currentPageIndex;
        private int _currentComamandIndex;
        private List<VariableBase> _variables;

        /// <summary>
        /// 再生中か
        /// </summary>
        public bool IsPlaying {get; private set;}

        public bool IsPaused {get; private set;}
        /// <summary>
        /// ポーズ
        /// </summary>
        public void Pause() => IsPaused = true;
        /// <summary>
        /// 再開
        /// </summary>
        public void Resume() => IsPaused = false;

        /// <summary>
        /// プリロード待機中
        /// </summary>
        /// <value></value>
        public bool WaitingPreload {get; private set;}

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        void Awake() {
            _scenarioCache = new ScenarioCache(PreloadAsync, Release);
            // 初期アタッチ済みシナリオをキャッシュに追加
            foreach(Scenario scenario in _scenarios) {
                _scenarioCache.AddAndPreload(scenario, false);
            }
            // 自動再生
            if(_playFirstScenarioOnAwake && _scenarios.Count > 0) {
                PlayScenario(_scenarioCache.Cache[0].Scenario.name);
            }
        }

        void OnDestroy() {
            _cts.Cancel();
            _scenarioCache.RemoveAndReleaseAll();
        }

        /// <summary>
        /// シナリオの追加
        /// </summary>
        /// <param name="scenario">シナリオ</param>
        /// <param name="removeOnExitScenario">シナリオ終了時に自動でRemoveするか</param>
        /// <param name="onRemove">Removeコールバック</param>
        public void AddScenario(Scenario scenario, bool removeOnExitScenario = true, Action onRemove = null) {
            _scenarioCache.AddAndPreload(scenario, removeOnExitScenario, onRemove);
        }

        /// <summary>
        /// シナリオを外す
        /// </summary>
        /// <param name="scenario"></param>
        public void RemoveScenario(Scenario scenario) {
            _scenarioCache.RemoveAndRelease(scenario);
        }

        /// <summary>
        /// シナリオ移動
        /// </summary>
        /// <param name="scenario"></param>
        private void ChangeCurrentScenario(Scenario scenario) {
            // ExitScenario
            ScenarioCacheData cacheData = _scenarioCache.Cache.FirstOrDefault(x => x.Scenario == _currentScenario);
            if(cacheData != null && cacheData.RemoveOnExitScenario) _scenarioCache.RemoveAndRelease(_currentScenario);

            // EnterScenario
            _currentScenario = scenario;
            _currentPageIndex = 0;
            _currentComamandIndex = 0;
            _variables = scenario?.Variables.Select(x => x.Copy()).ToList();
        }

        /// <summary>
        /// シナリオ同期実行
        /// </summary>
        public void PlayScenario(string scenarioName, CancellationToken cancellationToken = default) {
            var _ = PlayScenarioAsync(scenarioName, cancellationToken);
        }

        /// <summary>
        /// シナリオ非同期実行
        /// </summary>
        /// <param name="label"></param>
#if KODAMA_UNITASK_SUPPORT
        public async UniTask PlayScenarioAsync(string scenarioName, CancellationToken cancellationToken = default) {
#else
        public async Task PlayScenarioAsync(string scenarioName, CancellationToken cancellationToken = default) {
#endif
            // 排他制御
            if(IsPlaying) return;

            ScenarioCacheData cacheData = _scenarioCache.FindScenarioCacheDataByName(scenarioName);
            if(cacheData == null) {
                Debug.Log($"ScenarioEngine doesn't have scenario\"{scenarioName}\"");
                return;
            }
            ChangeCurrentScenario(cacheData.Scenario);
            await PlayAsyncInternal(cancellationToken);
        }

#if KODAMA_UNITASK_SUPPORT
        private async UniTask PlayAsyncInternal(CancellationToken cancellationToken = default) {
#else
        private async Task PlayAsyncInternal(CancellationToken cancellationToken = default) {
#endif
            CancellationToken linkedToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _cts.Token)
                .Token;

            // プリロード中なら待機
            ScenarioCacheData cacheData = _scenarioCache.Cache.FirstOrDefault(x => x.Scenario == _currentScenario);
            if(cacheData.PreloadState == PreloadState.Loading) {
                WaitingPreload = true;
#if KODAMA_UNITASK_SUPPORT
                await UniTask.WaitUntil(() => cacheData.PreloadState == PreloadState.Completed, cancellationToken: linkedToken);
#else
                while(cacheData.PreloadState == PreloadState.Completed) {
                    await Task.Delay(20, linkedToken);
                }
#endif
                WaitingPreload = false;
            }

            IsPlaying = true;
            IsPaused = false;

            while(true) {
                while(IsPlaying) {
                    if(_currentScenario == null) break;
                    if(_currentPageIndex >= _currentScenario.Pages.Count) break;
                    if(_currentComamandIndex >= _currentScenario.Pages[_currentPageIndex].Commands.Count) break;
                    
                    CommandBase command = _currentScenario.Pages[_currentPageIndex].Commands[_currentComamandIndex];

                    if (command is AsyncCommandBase asyncCommand) {
                        if (asyncCommand.Wait) {
                            await asyncCommand.ExecuteAsync(this, linkedToken);
                        }
                        else {
                            var _ = asyncCommand.ExecuteAsync(this, linkedToken);
                        }
                    }
                    else {
                        command.Execute(this);
                    }

                    // ポーズ
#if KODAMA_UNITASK_SUPPORT
                    await UniTask.WaitUntil(() => IsPaused == false, cancellationToken: linkedToken);
#else
                    while(IsPaused) {
                        await Task.Delay(20, linkedToken);
                    }
#endif
                    _currentComamandIndex++;
                }
                ChangeCurrentScenario(null);
                break;
            }

            IsPlaying = false;
        }

        /// <summary>
        /// ラベルへジャンプ
        /// </summary>
        private void JumpToLabel(string targetLabel) {
            // まず現在のページを調べる
            for(int i = 0; i < _currentScenario.Pages[_currentPageIndex].Commands.Count; i++) {
                LabelCommand labelCommand = _currentScenario.Pages[_currentPageIndex].Commands[i] as LabelCommand;
                if(labelCommand != null && labelCommand.Label == targetLabel) {
                    _currentComamandIndex = i;
                    return;
                }
            }
            // 現在のシナリオを調べる
            for(int i = 0; i < _currentScenario.Pages.Count; i++) {
                for(int j = 0; j < _currentScenario.Pages[i].Commands.Count; j++) {
                    LabelCommand labelCommand = _currentScenario.Pages[i].Commands[j] as LabelCommand;
                    if(labelCommand != null && labelCommand.Label == targetLabel) {
                        _currentPageIndex = i;
                        _currentComamandIndex = j;
                        return;
                    }
                }
            }
            Debug.LogWarning($"{nameof(JumpToLabel)} : target label not found.");
        }

        /// <summary>
        /// ラベルへジャンプ(アセンブリ内公開用)
        /// </summary>
        void IScenarioEngineForInternal.JumpToLabel(string targetLabel) {
            JumpToLabel(targetLabel);
        }

#region Variable
        /// <summary>
        /// 指定した型、名前の変数の値を取得
        /// </summary>
        public T GetVariableValue<T>(string variableName) {
            var castedVariables = _variables.OfType<Variable<T>>();
            if(castedVariables.Count() == 0) {
                Debug.LogError($"{typeof(T).Name} variable not found.");
            }
            var variable = castedVariables.Where(x => x.Name == variableName).FirstOrDefault();
            if(variable == null) {
                Debug.LogError($"variable (name = {variableName}) not found.");
            }
            return variable.Value;
        }

        /// <summary>
        /// 指定した型、名前の変数の値を設定
        /// </summary>
        public void SetVariableValue<T>(string variableName, T value) {
            var castedVariables = _variables.OfType<Variable<T>>();
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

#region Preload/Release
        /// <summary>
        /// リソースプリロード
        /// </summary>
#if KODAMA_UNITASK_SUPPORT
        internal async UniTask PreloadAsync(Scenario scenario, CancellationToken cancellationToken) {
#else
        internal async Task PreloadAsync(Scenario scenario, CancellationToken cancellationToken) {
#endif
            foreach(CommandBase command in scenario.Pages.SelectMany(x => x.Commands)) {
                cancellationToken.ThrowIfCancellationRequested();
                if(command is IPreloadable preloadable) {
                    await preloadable.PreloadAsync(this);
                }
            }
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        internal void Release(Scenario scenario) {
            foreach(CommandBase command in scenario.Pages.SelectMany(x => x.Commands)) {
                if(command is IPreloadable preloadable) {
                    preloadable.Release(this);
                }
            }
        }
#endregion

#region ServiceLocator
        /// <summary>
        /// 参照解決
        /// </summary>
        public T Resolve<T>() {
            return _componentBindings.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 参照解決(全件)
        /// </summary>
        public IEnumerable<T> ResolveAll<T>() {
            return _componentBindings.OfType<T>();
        }
#endregion
    }
}