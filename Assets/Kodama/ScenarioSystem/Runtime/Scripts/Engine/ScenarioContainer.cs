using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif
namespace Kodama.ScenarioSystem {
    internal enum PreloadState {
        Yet,
        Loading,
        Completed,
    }
    internal class ScenarioCacheData {

        internal Scenario Scenario {get;}
        internal bool RemoveOnExitScenario {get;}
        internal Action OnRemove {get;}
        internal PreloadState PreloadState {get; set;} = PreloadState.Yet;
        private readonly CancellationTokenSource preloadCts = new CancellationTokenSource();
        internal ScenarioCacheData(Scenario scenario, bool removeOnExitScenario, Action onRemove) {
            Scenario = scenario;
            RemoveOnExitScenario = removeOnExitScenario;
            OnRemove = onRemove;
        }
#if KODAMA_UNITASK_SUPPORT
        internal async UniTask PreloadAsync(Func<Scenario, CancellationToken, UniTask> preloadMethod) {
#else
        internal async Task PreloadAsync(Func<Scenario, CancellationToken, Task> preloadMethod) {
#endif
            PreloadState = PreloadState.Loading;
            await preloadMethod.Invoke(Scenario, preloadCts.Token);
            PreloadState = PreloadState.Completed;
        }

        internal void ForceRelease(Action<Scenario> releaseMethod) {
            preloadCts.Cancel();
            releaseMethod.Invoke(Scenario);
        }
    }

    internal class ScenarioCache {
        private readonly List<ScenarioCacheData> _cache = new List<ScenarioCacheData>();
        internal IReadOnlyList<ScenarioCacheData> Cache => _cache;

#if KODAMA_UNITASK_SUPPORT
        private readonly Func<Scenario, CancellationToken, UniTask> _preloadMethod;
#else
        private readonly Func<Scenario, CancellationToken, Task> _preloadMethod;
#endif

        private readonly Action<Scenario> _releaseMethod;

#if KODAMA_UNITASK_SUPPORT
        internal ScenarioCache(Func<Scenario, CancellationToken, UniTask> preloadMethod, Action<Scenario> releaseMethod) {
#else
        internal ScenarioCache(Func<Scenario, CancellationToken, Task> preloadMethod, Action<Scenario> releaseMethod) {
#endif
            _preloadMethod = preloadMethod;
            _releaseMethod = releaseMethod;
        }

        internal ScenarioCacheData FindScenarioCacheDataByName(string scenarioName) => Cache.FirstOrDefault(x => x.Scenario.name == scenarioName);

        /// <summary>
        /// シナリオをキャッシュに追加
        /// </summary>
        internal void AddAndPreload(Scenario scenario, bool removeOnExitScenario = true, Action onRemove = null) {
            ScenarioCacheData cacheData = new ScenarioCacheData(scenario, removeOnExitScenario, onRemove);
            _cache.Add(cacheData);
            var _ = cacheData.PreloadAsync(_preloadMethod);
        }

        /// <summary>
        /// シナリオをキャッシュから削除
        /// </summary>
        internal void RemoveAndRelease(Scenario scenario) {
                ScenarioCacheData cacheData = _cache.Find(x => x.Scenario == scenario);
                if(cacheData == null) {
                    Debug.Log($"scenario\"{scenario.name}\" not found.");
                    return;
                }
                cacheData.ForceRelease(_releaseMethod);
                cacheData.OnRemove?.Invoke();
                _cache.Remove(cacheData);
        }

        /// <summary>
        /// シナリオを全削除
        /// </summary>
        internal void RemoveAndReleaseAll() {
            foreach(ScenarioCacheData cacheData in _cache.ToArray()) {
                RemoveAndRelease(cacheData.Scenario);
            }
        }
    }
}