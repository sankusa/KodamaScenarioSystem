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
            return $"<b>{_label}</b>";
        }
    }
}