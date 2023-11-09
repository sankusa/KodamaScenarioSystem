using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class Variable<T> : VariableBase {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private T _value;
        public T Value => _value;
    }
}