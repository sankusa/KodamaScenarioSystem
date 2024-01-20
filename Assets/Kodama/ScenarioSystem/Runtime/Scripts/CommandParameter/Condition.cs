using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public static class CompareOperatorExtension {
        public static string GetOperatorString(this Condition.CompareOperator compareOperator) {
            if(compareOperator == Condition.CompareOperator.EqualTo) return "==";
            if(compareOperator == Condition.CompareOperator.NotEqualTo) return "!=";
            if(compareOperator == Condition.CompareOperator.LessThan) return "<";
            if(compareOperator == Condition.CompareOperator.GreaterThan) return ">";
            if(compareOperator == Condition.CompareOperator.LessThanOrEqualTo) return "<=";
            if(compareOperator == Condition.CompareOperator.GreaterThanOrEqualTo) return ">=";
            return "";
        }
    }

    [Serializable]
    public class Condition {
        public enum CompareOperator {
            EqualTo,
            NotEqualTo,
            LessThan,
            GreaterThan,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo,
        }

        public static CompareOperator[] OperatorsForCompareable = new CompareOperator[]{
            CompareOperator.EqualTo,
            CompareOperator.NotEqualTo,
            CompareOperator.LessThan,
            CompareOperator.GreaterThan,
            CompareOperator.LessThanOrEqualTo,
            CompareOperator.GreaterThanOrEqualTo,
        };

        public static CompareOperator[] OperatorsForNotCompareable = new CompareOperator[]{
            CompareOperator.EqualTo,
            CompareOperator.NotEqualTo,
        };

        [SerializeReference] private VariableName _variableName;
        public VariableName VariableName {
            get => _variableName;
            set => _variableName = value;
        }
        [SerializeReference] private ValueOrVariableName _valueOrVariableName;
        public ValueOrVariableName ValueOrVariableName {
            get => _valueOrVariableName;
            set => _valueOrVariableName = value;
        }
        [SerializeField] private CompareOperator _operator = CompareOperator.EqualTo;
        public CompareOperator Operator {
            get => _operator;
            set => _operator = value;
        }

        public Condition() {
            _variableName = new IntVariableName();
            _valueOrVariableName = new IntValueOrVariableName();
        }

        public bool Evaluate(IPagePlayProcess process) {
            object value1 = process.GetVariableValue(_variableName.TargetType, _variableName.Name);
            object value2 = string.IsNullOrEmpty(_valueOrVariableName.VariableName)
                ? _valueOrVariableName.GetValueAsObject()
                : process.GetVariableValue(_valueOrVariableName.TargetType, _valueOrVariableName.VariableName);

            if(_operator == CompareOperator.EqualTo) return value1.Equals(value2);
            else if(_operator == CompareOperator.NotEqualTo) return !value1.Equals(value2);
            else if(_operator == CompareOperator.LessThan) return ((IComparable)value1).CompareTo((IComparable)value2) < 0;
            else if(_operator == CompareOperator.GreaterThan) return ((IComparable)value1).CompareTo((IComparable)value2) > 0;
            else if(_operator == CompareOperator.LessThanOrEqualTo) return ((IComparable)value1).CompareTo((IComparable)value2) <= 0;
            else if(_operator == CompareOperator.GreaterThanOrEqualTo) return ((IComparable)value1).CompareTo((IComparable)value2) >= 0;

            return false;
        }

        public string GetSummary() {
            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append(_variableName.GetSummary());
            sb.Append("  ");
            sb.Append(_operator.GetOperatorString());
            sb.Append("  ");
            sb.Append(_valueOrVariableName.GetSummary());
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public string Validate(CommandBase parentCommand) {
            StringBuilder sb = SharedStringBuilder.Instance;
            string variableNameErrorMessage = _variableName.Validate(parentCommand);
            string valueOrVariableNameErrorMessage = _valueOrVariableName.Validate(parentCommand);
            sb.Append(variableNameErrorMessage);
            if(string.IsNullOrEmpty(variableNameErrorMessage) == false && string.IsNullOrEmpty(valueOrVariableNameErrorMessage) == false) {
                sb.Append("\n");
            }
            sb.Append(valueOrVariableNameErrorMessage);
            string errorMessage = sb.ToString();
            sb.Clear();

            return errorMessage;
        }
    }
}