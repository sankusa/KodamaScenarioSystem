using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class SiblingPageSelector {
        [SerializeField] private ScenarioPage _page;
        public ScenarioPage Page => _page;

        public string GetSummary() {
            return _page != null ? _page.name : Labels.Label_DefaultPage_Dark;
        }

        public string Validate(CommandBase parentCommand, string label) {
            if(_page == null) return "";
            if(parentCommand.ParentPage.IsSiblig(_page) == false) return label + " : Page is not sibling";
            return "";
        }
    }
}