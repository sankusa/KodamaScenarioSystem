using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class GoToLabelCommand : CommandBase {
        [SerializeField] private string _targetLabel;
        public string TargetLabel => _targetLabel;

        public override void Execute(ICommandService service) {
            int i;
            for(i = 0; i < ParentPage.Commands.Count; i++) {
                if(ParentPage.Commands[i] is LabelCommand labelCommand && labelCommand.Label == _targetLabel) break;
            }
            service.PageProcess.JumpToIndex(i);
        }

        public override string GetSummary() {
            SharedStringBuilder.Append("<b>");
            SharedStringBuilder.Append(_targetLabel);
            SharedStringBuilder.Append("</b>");
            return SharedStringBuilder.Output();
        }

        public override string Validate() {
            if(ParentPage.Commands.OfType<LabelCommand>().Any(x => x.Label == _targetLabel) == false) {
                return "Target label not found in this page";
            }
            return null;
        }
    }
}