using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codice.Client.Commands;
using UnityEngine.Assertions;

namespace Kodama.ScenarioSystem {
    internal class ScenarioCache {
        internal class ScenarioCacheElement {
            public Scenario Scenario {get;}
            public bool RemoveOnExitScenario {get;}
            public Action OnRemove {get;}

            public ScenarioCacheElement(Scenario scenario, bool removeOnExitScenario = false, Action onRemove = null) {
                Assert.IsTrue(scenario != null);

                Scenario = scenario;
                RemoveOnExitScenario = removeOnExitScenario;
                OnRemove = onRemove;
            }
        }

        private readonly List<ScenarioCacheElement> _elements = new List<ScenarioCacheElement>();
        public IReadOnlyList<ScenarioCacheElement> Elements => _elements;

        public IEnumerable<Scenario> Scenarios => _elements.Select(x => x.Scenario);

        public event Action<Scenario> OnAdd;
        public event Action<Scenario> OnRmove;

        public ScenarioCacheElement Find(Scenario scenario) => _elements.Find(x => x.Scenario == scenario);

        public Scenario FindScenarioByName(string scenarioName) => _elements.Find(x => x.Scenario.name == scenarioName).Scenario;
        
        public void Add(Scenario scenario, bool removeOnExitScenario = false, Action onRemove = null) {
            _elements.Add(
                new ScenarioCacheElement(
                    scenario,
                    removeOnExitScenario,
                    onRemove
                )
            );
            OnAdd.Invoke(scenario);
        }

        public void Remove(Scenario scenario) {
            ScenarioCacheElement removeTarget = _elements.Find(x => x.Scenario == scenario);
            Remove(removeTarget);
        }

        private void Remove(ScenarioCacheElement element) {
            if(element != null) {
                _elements.Remove(element);
                OnRmove.Invoke(element.Scenario);
                element.OnRemove?.Invoke();
            }
        }

        public void RemoveAll() {
            for(int i = Elements.Count - 1; i >= 0; i--) {
                Remove(Elements[i]);
            }
        }
    }
}