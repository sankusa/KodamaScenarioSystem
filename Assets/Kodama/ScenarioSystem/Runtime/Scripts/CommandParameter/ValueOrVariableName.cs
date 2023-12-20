using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class ValueOrVariableName {
        [SerializeField] protected string _variableName;
        public string VariableName => _variableName;

        public abstract object GetValueAsObject();

        public abstract Type TargetType {get;}
        public abstract string GetSummary();
    }
    [Serializable]
    public class ValueOrVariableName<T> : ValueOrVariableName {
        [SerializeField] private T _value;
        public T Value => _value;

        public override object GetValueAsObject() => _value;

        public override Type TargetType => typeof(T);
        public override string GetSummary()
        => string.IsNullOrEmpty(_variableName) ? _value?.ToString() : $"<i>{_variableName}</i>";
    }
}