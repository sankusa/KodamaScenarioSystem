using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LabelCommand : CommandBase {
        [SerializeField] private string _label;
        public string Label => _label;

        public override string GetSummary() {
            return $"<color={Colors.BasicSummaryCaption}>Label [ <color={Colors.Args}>{_label}</color> ]</color>";
        }
    }
}