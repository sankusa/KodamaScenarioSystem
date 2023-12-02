using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPreloadManager {
        private enum PreloadState {
            Yet,
            Loading,
            Completed,
        }

        private class Preload {
            public Scenario Scenario {get;}
            public PreloadState PreloadState {get; private set;} = PreloadState.Yet;
            private CancellationTokenSource PreloadCts {get;} = new CancellationTokenSource();

            public Preload(Scenario scenario) {
                Scenario = scenario;
            }

            public async UniTask PreloadAsync(ScenarioEngine engine) {
                if(PreloadState != PreloadState.Yet) return;

                PreloadState = PreloadState.Loading;
                
                try {
                    foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                        PreloadCts.Token.ThrowIfCancellationRequested();
                        if(command is IPreloadable preloadable) {
                            await preloadable.PreloadAsync(engine);
                        }
                    }
                }
                catch(OperationCanceledException) {}
                catch(Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                PreloadState = PreloadState.Completed;
            }

            public void ForceRelease(ScenarioEngine engine) {
                // プリロードキャンセル
                PreloadCts.Cancel();

                // リリース処理呼び出し
                foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                    if(command is IPreloadable preloadable) {
                        preloadable.Release(engine);
                    }
                }
            }
        }

        private List<Preload> _preloads = new List<Preload>();

        private ScenarioEngine _engine;

        public ScenarioPreloadManager(ScenarioEngine engine, ScenarioCache cache) {
            _engine = engine;
            cache.OnAdd += OnAddScenario;
            cache.OnRmove += OnRemoveScenario;
        }

        private void OnAddScenario(Scenario scenario) {
            Preload preload = new Preload(scenario);
            _preloads.Add(preload);
            preload.PreloadAsync(_engine).Forget();
        }

        private void OnRemoveScenario(Scenario scenario) {
            Preload preload = _preloads.Find(x => x.Scenario == scenario);
            preload.ForceRelease(_engine);
            _preloads.Remove(preload);
        }

        public bool IsPreloading(Scenario scenario) {
            return _preloads.Find(x => x.Scenario == scenario).PreloadState != PreloadState.Completed;
        }
    }
}