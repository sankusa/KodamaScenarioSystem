using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LabelCommand : CommandBase {
        [SerializeField, TextArea] private string _label;
        public string Label => _label;

        public override string GetSummary()
        {
            return $"<color=#22BB22>ラベル</color> : <color=#AAAAFF>{_label}</color>";
        }
    }
}