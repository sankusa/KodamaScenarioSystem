using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ScenarioAndChildPageSelector {
        [SerializeField] private Scenario _scenario;
        public Scenario Scenario => _scenario;
        [SerializeField] private ScenarioPage _page;
        public ScenarioPage Page => _page;

        public string GetSummary() {
            SharedStringBuilder.Append(_scenario != null ? _scenario.name : Labels.Label_Null_Red);
            SharedStringBuilder.Append(",  ");
            SharedStringBuilder.Append(_page != null ? _page.name : Labels.Label_DefaultPage_Dark);
            return SharedStringBuilder.Output();
        }

        public string Validate(string label) {
            if(_scenario == null) {
                return $"{label} : Scenario is null";
            }
            else if(_page != null && _scenario.Pages.Contains(_page) == false) {
                return $"{label} : Page is not child of scenario";
            }
            return "";
        }
    }
}