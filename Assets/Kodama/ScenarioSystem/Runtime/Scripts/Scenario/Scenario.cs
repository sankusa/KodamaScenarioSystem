using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [CreateAssetMenu(fileName = "New " + nameof(Scenario), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(Scenario))]
    public class Scenario : ScriptableObject {
        [SerializeField] private List<ScenarioPage> _pages;
        public IReadOnlyList<ScenarioPage> Pages => _pages;
        [SerializeReference] private List<VariableBase> _variables;
        public IList<VariableBase> Variables => _variables;
    }
}