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
            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append("<b>");
            sb.Append(_label);
            sb.Append("</b>");
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }
    }
}