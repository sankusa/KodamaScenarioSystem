using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class AsyncCommandSetting {
        [SerializeField] private bool _wait = true;
        public bool Wait => _wait;
        [SerializeField] private VariableKey<UniTask> _setUniTaskTo;
        public VariableKey<UniTask> SetUniTaskTo => _setUniTaskTo;

        public string Validate(CommandBase parentCommand, string label) {
            return _setUniTaskTo.Validate(parentCommand, false, label + " : SetUniTaskTo");
        }
    }
}