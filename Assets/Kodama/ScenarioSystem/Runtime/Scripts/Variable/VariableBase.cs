using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IVariableValueHolder {}

    [Serializable]
    public abstract class VariableBase : IVariableValueHolder {
        public abstract string Id {get;}
        public abstract string Name {get;set;}
        public abstract object GetValueAsObject();
        public abstract void SetValueAsObject(object obj);
        public abstract void SetValue(IVariableValueHolder valueHolder);
        public abstract Type TargetType {get;}
        public bool IsMatch(Type targetType, string id) {
            return TargetType == targetType && Id == id;
        }
        // 演算
        public virtual bool IsValidArthmeticOperator(AssignOperator assignOperator) => false;
        public abstract void Negate(object value);
        public abstract void Add(object value);
        public abstract void Subtract(object value);
        public abstract void Multiply(object value);
        public abstract void Divide(object value);
        public abstract void Remind(object value);

        internal abstract VariableBase Copy();
    }

    public enum AssignOperator {
        Assign,
        Negate,
        Add,
        Subtract,
        Multiply,
        Divide,
        Remind,
    }

    public static class AssignOperatorOperatorExtension {
        public static string GetOperatorString(this AssignOperator assignOperator) {
            if(assignOperator == AssignOperator.Assign) return "=";
            else if(assignOperator == AssignOperator.Negate) return "=!";
            else if(assignOperator == AssignOperator.Add) return "+=";
            else if(assignOperator == AssignOperator.Subtract) return "-=";
            else if(assignOperator == AssignOperator.Multiply) return "*=";
            else if(assignOperator == AssignOperator.Divide) return '\u2215' + "=";
            else if(assignOperator == AssignOperator.Remind) return "%=";
            return "";
        }
    }
}