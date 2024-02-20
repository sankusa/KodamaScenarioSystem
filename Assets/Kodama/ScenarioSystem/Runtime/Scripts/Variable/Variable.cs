using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IVariableValueHolder<T> : IVariableValueHolder {
        T Value {get; set;}
    }

    /// <summary>
    /// Variableを特定するキー情報
    /// </summary>
    public interface IVariableKey<T> : IVariableKey {}

    [Serializable]
    public abstract class Variable<T> : VariableBase, IVariableValueHolder<T> {
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

        public override void SetValue(IVariableValueHolder valueHolder) {
            IVariableValueHolder<T> genericValueHolder = valueHolder as IVariableValueHolder<T>;
            _value = genericValueHolder.Value;
        }

        public override Type TargetType => typeof(T);

        // 演算
        public sealed override void Negate(IVariableValueHolder value) => Negate(value as IVariableValueHolder<T>);
        public virtual void Negate(IVariableValueHolder<T> value) => throw new InvalidOperationException();
        public sealed override void Add(IVariableValueHolder value) => Add(value as IVariableValueHolder<T>);
        public virtual void Add(IVariableValueHolder<T> value) => throw new InvalidOperationException();
        public sealed override void Subtract(IVariableValueHolder value) => Subtract(value as IVariableValueHolder<T>);
        public virtual void Subtract(IVariableValueHolder<T> value) => throw new InvalidOperationException();
        public sealed override void Multiply(IVariableValueHolder value) => Multiply(value as IVariableValueHolder<T>);
        public virtual void Multiply(IVariableValueHolder<T> value) => throw new InvalidOperationException();
        public sealed override void Divide(IVariableValueHolder value) => Divide(value as IVariableValueHolder<T>);
        public virtual void Divide(IVariableValueHolder<T> value) => throw new InvalidOperationException();
        public sealed override void Remind(IVariableValueHolder value) => Remind(value as IVariableValueHolder<T>);
        public virtual void Remind(IVariableValueHolder<T> value) => throw new InvalidOperationException();

        internal override VariableBase Copy() {
            Variable<T> copied = (Variable<T>)JsonUtility.FromJson(JsonUtility.ToJson(this), GetType());
            copied._value = this.Value; // シリアライズ不可な型の考慮
            return copied;
        }
    }
}