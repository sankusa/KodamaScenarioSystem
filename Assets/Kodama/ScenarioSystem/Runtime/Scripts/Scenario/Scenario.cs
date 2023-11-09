using System.Collections;
using System.Collections.Generic;
using log4net.Layout.Pattern;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [CreateAssetMenu(fileName = "New " + nameof(Scenario), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(Scenario))]
    public class Scenario : ScriptableObject
    {
        [SerializeField] private List<ScenarioPage> _pages;
        public IReadOnlyList<ScenarioPage> Pages => _pages;
    }
}