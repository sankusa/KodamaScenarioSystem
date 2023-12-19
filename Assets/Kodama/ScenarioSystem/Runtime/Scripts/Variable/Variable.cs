using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class Variable<T> : VariableBase {
        public const string VariableName_Value = nameof(_value);
        
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

        public override object GetValueAsObject() {
            return _value;
        }

        public override void SetValueAsObject(object obj) {
            _value = (T)obj;
        }

        public override Type TargetType => typeof(T);

        internal override VariableBase Copy() {
            return new Variable<T>(){_name = this.Name, _value = this.Value};
        }
    }
}