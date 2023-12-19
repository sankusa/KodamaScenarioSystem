using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class Variable<T> : VariableBase {
        [SerializeField] private string _name;
        public override string Name {
            get => _name;
            set => _name = value;
        }
        [SerializeField] private T _value;
        public T Value {
            get => _value;
            set => _value = value;
        }

        internal override VariableBase Copy() {
            return new Variable<T>(){_name = this.Name, _value = this.Value};
        }
    }
}