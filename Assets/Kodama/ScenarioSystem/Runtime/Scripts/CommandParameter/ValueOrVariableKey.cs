using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kodama.ScenarioSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class ValueOrVariableKey {
        public abstract VariableKey VariableKey {get;}

        public bool HasKey() => VariableKey.IsEmpty() == false;

        public abstract object GetValueAsObject();

        public abstract Type TargetType {get;}
        public abstract string GetSummary(CommandBase parentCommand);
        public abstract string Validate(CommandBase parentCommand, string name = null);
    }

    [Serializable]
    public class ValueOrVariableKey<T> : ValueOrVariableKey {
        [SerializeField] private VariableKey<T> _variableKey;
        public override VariableKey VariableKey => _variableKey;

        [SerializeField] private T _value;
        public T Value => _value;

        public override object GetValueAsObject() => _value;

        public override Type TargetType => typeof(T);

        public override string GetSummary(CommandBase parentCommand) {
            if(_variableKey.IsEmpty()) return _value.ToString();
            return _variableKey.GetSummary(parentCommand, false);
        }

        public override string Validate(CommandBase parentCommand, string name = null) {
            if(string.IsNullOrEmpty(name)) name = nameof(ValueOrVariableKey);
            return _variableKey.Validate(parentCommand, false, name);
        }
    }
}