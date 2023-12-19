using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal enum PreloadState {
        Yet,
        Preloading,
        Completed,
    }
    internal class AvailableScenario {
        public Scenario Scenario {get; set;}

        public bool RemoveOnExitScenario {get;}
        public Action OnRemove {get;}

        public PreloadState PreloadState {get; private set;} = PreloadState.Yet;
        private CancellationTokenSource PreloadCts {get;} = new CancellationTokenSource();

        public AvailableScenario(Scenario scenario, bool removeOnExitScenario = false, Action onRemove = null) {
            Scenario = scenario;
            RemoveOnExitScenario = removeOnExitScenario;
            OnRemove = onRemove;
        }

        public async UniTask PreloadAsync() {
            if(PreloadState != PreloadState.Yet) return;

            PreloadState = PreloadState.Preloading;
            
            foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                PreloadCts.Token.ThrowIfCancellationRequested();
                if(command is IPreloadable preloadable) {
                    await preloadable.PreloadAsync();
                }
            }
            
            PreloadState = PreloadState.Completed;
        }

        public void ForceRelease() {
            PreloadCts.Cancel();

            foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                if(command is IPreloadable preloadable) {
                    preloadable.Release();
                }
            }
        }
    }
    internal class ScenarioManager {
        private static ScenarioManager _instance;
        public static ScenarioManager Instance {
            get {
                if(_instance == null) {
                    _instance = new ScenarioManager();
                }
                return _instance;
            }
        }

        public List<AvailableScenario> Availables {get;} = new List<AvailableScenario>();

        public AvailableScenario FindAvailable(Scenario scenario) => Availables.Find(x => x.Scenario == scenario);
        public AvailableScenario FindAvailableByName(string scenarioName) => Availables.Find(x => x.Scenario.name == scenarioName);

        public void Add(Scenario scenario, bool removeOnExitScenario = false, Action onRemove = null) {
            AvailableScenario available = new AvailableScenario(scenario, removeOnExitScenario, onRemove);
            Availables.Add(available);
            available.PreloadAsync().Forget(e => Debug.LogError(e));
        }

        public void Remove(Scenario scenario) {
            AvailableScenario available = Availables.Find(x => x.Scenario == scenario);
            Remove(available);
        }

        public void Remove(AvailableScenario available) {
            if(available != null) {
                Availables.Remove(available);
                available.ForceRelease();
                available.OnRemove?.Invoke();
            }
        }

        public void RemoveAll() {
            for(int i = Availables.Count - 1; i >= 0; i--) {
                Remove(Availables[i]);
            }
        }
    }
}