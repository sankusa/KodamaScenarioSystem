using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LogCommand : CommandBase {
        [SerializeField] private string _message;
        public string Message => _message;

        public override void Execute(IScenarioEngine engine) {
            Debug.Log(_message);
        }
    }
}