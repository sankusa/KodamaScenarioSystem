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
    public abstract class ValueOrVariableKey : IVariableValueHolder {
        public abstract VariableKey VariableKey {get;}

        public bool HasKey() => VariableKey.IsEmpty() == false;

        public abstract object GetValueAsObject();
        public object ResolveValueAsObject(IPagePlayProcess pageProcess) {
            return HasKey()
                ? pageProcess.FindVariable(VariableKey).GetValueAsObject()
                : GetValueAsObject();
        }
        public IVariableValueHolder ResolveValueAsVariableValueHolder(IPagePlayProcess pageProcess) {
            return HasKey()
                ? pageProcess.FindVariable(VariableKey) as IVariableValueHolder
                : this as IVariableValueHolder;
        }
        public abstract Type TargetType {get;}
        public abstract string GetSummary(CommandBase parentCommand);
        public abstract string Validate(CommandBase parentCommand, string label = null);
    }

    [Serializable]
    public class ValueOrVariableKey<T> : ValueOrVariableKey, IVariableValueHolder<T> {
        [SerializeField] private VariableKey<T> _variableKey;
        public override VariableKey VariableKey => _variableKey;

        [SerializeField] private T _value;
        public T Value {
            get => _value;
            set => _value = value;
        }

        public override object GetValueAsObject() => _value;

        public T ResolveValue(IPagePlayProcess pageProcess) {
            return HasKey()
                ? pageProcess.FindVariable(_variableKey).Value
                : _value;
        }

        public override Type TargetType => typeof(T);

        public override string GetSummary(CommandBase parentCommand) {
            if(_variableKey.IsEmpty()) return _value?.ToString();
            return _variableKey.GetSummary(parentCommand, false);
        }

        public override string Validate(CommandBase parentCommand, string label = null) {
            if(string.IsNullOrEmpty(label)) label = nameof(ValueOrVariableKey);
            return _variableKey.Validate(parentCommand, false, label + " : " + nameof(VariableKey));
        }
    }
}