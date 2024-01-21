using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class AsyncCommandWaitSetting {
        [SerializeField] private bool _wait = true;
        public bool Wait => _wait;
        [SerializeField] private UniTaskVariableName _returnValueSetTarget;
        public UniTaskVariableName ReturnValueSetTarget => _returnValueSetTarget;
    }
}