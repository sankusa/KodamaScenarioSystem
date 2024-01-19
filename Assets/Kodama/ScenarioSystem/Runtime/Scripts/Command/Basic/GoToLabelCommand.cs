using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class GoToLabelCommand : CommandBase {
        [SerializeField] private string _targetLabel;
        public string TargetLabel => _targetLabel;

        public override void Execute(ICommandService service) {
            int i;
            for(i = 0; i < Page.Commands.Count; i++) {
                if(Page.Commands[i] is LabelCommand labelCommand && labelCommand.Label == _targetLabel) break;
            }
            service.PagePlayProcess.JumpToIndex(i);
        }

        public override string GetSummary() {
            return $"<b>{_targetLabel}</b>";
        }
    }
}