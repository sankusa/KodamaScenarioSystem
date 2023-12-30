using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LogCommand : CommandBase {
        [SerializeField] private string _message;
        public string Message => _message;

        public override void Execute(ICommandService service) {
            Debug.Log($"{LogHeader}\n{_message}", Page.Scenario);
        }

        public override string GetSummary() {
            return $"<color={Colors.CommandNameColor}>Log [ <color={Colors.Args}>{_message}</color> ]</color>";
        }
    }
}