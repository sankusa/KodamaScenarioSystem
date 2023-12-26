using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class JumpToLabelCommand : CommandBase {
        [SerializeField] private string _targetLabel;
        public string TargetLabel => _targetLabel;

        public override void Execute(ICommandService service) {
            service.PagePlayProcess.JumpToLabel(_targetLabel);
        }
    }
}