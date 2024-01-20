using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class LogWarningCommand : CommandBase {
        [SerializeField, TextArea(1, 10)] private string _message;

        public override void Execute(ICommandService service) {
            Debug.LogWarning($"{LogHeader}\n{_message}", ParentPage.ParentScenario);
        }

        public override string GetSummary() => _message;
    }
}