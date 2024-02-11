using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class Variable<T> : VariableBase {
        public const string VariableName_Value = nameof(_value);
        
        [SerializeField] private string _id;
        public override string Id => _id;

        [SerializeField] private string _name;
        public override string Name {
            get => _name;
            set => _name = value;
        }
        
        [SerializeField] protected T _value;
        public T Value {
            get => _value;
            set => _value = value;
        }

        public Variable() {
            _id = Guid.NewGuid().ToString("N");
        }

        public override object GetValueAsObject() {
            return _value;
        }

        public override void SetValueAsObject(object obj) {
            _value = (T)obj;
        }

        public override Type TargetType => typeof(T);

        // 演算
        public sealed override void Negate(object value) => Negate((T)value);
        public virtual void Negate(T value) => throw new InvalidOperationException();
        public sealed override void Add(object value) => Add((T)value);
        public virtual void Add(T value) => throw new InvalidOperationException();
        public sealed override void Subtract(object value) => Subtract((T)value);
        public virtual void Subtract(T value) => throw new InvalidOperationException();
        public sealed override void Multiply(object value) => Multiply((T)value);
        public virtual void Multiply(T value) => throw new InvalidOperationException();
        public sealed override void Divide(object value) => Divide((T)value);
        public virtual void Divide(T value) => throw new InvalidOperationException();
        public sealed override void Remind(object value) => Remind((T)value);
        public virtual void Remind(T value) => throw new InvalidOperationException();

        internal override VariableBase Copy() {
            Variable<T> copied = (Variable<T>)JsonUtility.FromJson(JsonUtility.ToJson(this), GetType());
            copied._value = this.Value; // シリアライズ不可な型の考慮
            return copied;
        }
    }
}