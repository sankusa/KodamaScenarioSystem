using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class SetVariableCommand : CommandBase {
        [SerializeField] private VariableSetter _variableSetter;

        public override void Execute(ICommandService service) {
            _variableSetter.Set(service.PageProcess);
        }

        public override string GetSummary() {
            return _variableSetter.GetSummary(this);
        }

        public override string Validate() {
            return _variableSetter.Validate(this);
        }
    }
}