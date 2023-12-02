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
            Debug.Log(_message);
        }

        public override string GetSummary() {
            return $"<b><color=#22BB22>ログ出力</color></b> : <color=#AAAAFF>{_message}</color>";
        }
    }
}