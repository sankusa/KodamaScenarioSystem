using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LogCommand : CommandBase {
        [SerializeField, TextArea(1, 10)] private string _message;
        public string Message => _message;

        public override void Execute(ICommandService service) {
            Debug.Log($"{LogHeader}\n{_message}", Page.Scenario);
        }

        public override string GetSummary() {
            return _message;
        }
    }
}