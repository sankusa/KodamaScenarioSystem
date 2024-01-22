using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class LabelCommand : CommandBase {
        [SerializeField] private string _label;
        public string Label => _label;

        public override string GetSummary() {
            SharedStringBuilder.Append("<b>");
            SharedStringBuilder.Append(_label);
            SharedStringBuilder.Append("</b>");
            return SharedStringBuilder.Output();
        }
    }
}