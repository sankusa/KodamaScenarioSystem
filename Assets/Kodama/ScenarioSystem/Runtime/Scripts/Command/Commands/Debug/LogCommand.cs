using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class LogCommand : CommandBase {
        [SerializeField, TextArea(1, 10)] private string _message;

        public override void Execute(ICommandService service) {
            Debug.Log($"{LogHeader}\n{_message}", ParentPage.ParentScenario);
        }

        public override string GetSummary() => _message;
    }
}