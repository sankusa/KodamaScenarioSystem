using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class CommandParameter<T> {
        [SerializeField] private string _variableName;
        public string VariableName => _variableName;

        [SerializeField] private T _value;
        public T Value => _value;
    }
}