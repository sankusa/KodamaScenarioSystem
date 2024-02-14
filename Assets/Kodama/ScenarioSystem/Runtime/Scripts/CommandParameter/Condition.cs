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

        [SerializeReference] private VariableKey _variableKey;
        public VariableKey VariableKey {
            get => _variableKey;
            set => _variableKey = value;
        }
        [SerializeReference] private ValueOrVariableKey _valueOrVariableKey;
        public ValueOrVariableKey ValueOrVariableKey {
            get => _valueOrVariableKey;
            set => _valueOrVariableKey = value;
        }
        [SerializeField] private CompareOperator _operator = CompareOperator.EqualTo;
        public CompareOperator Operator {
            get => _operator;
            set => _operator = value;
        }

        public Condition() {
            // _variableKey = new IntVariableKey();
            // _valueOrVariableKey = new IntValueOrVariableKey();
        }

        public bool Evaluate(IPagePlayProcess process) {
            object value1 = process.FindVariable(_variableKey.TargetType, _variableKey.Id).GetValueAsObject();
            object value2 = _valueOrVariableKey.HasKey()
                ? _valueOrVariableKey.GetValueAsObject()
                : process.FindVariable(_valueOrVariableKey.VariableKey).GetValueAsObject();

            if(_operator == CompareOperator.EqualTo) return value1.Equals(value2);
            else if(_operator == CompareOperator.NotEqualTo) return !value1.Equals(value2);
            else if(_operator == CompareOperator.LessThan) return ((IComparable)value1).CompareTo((IComparable)value2) < 0;
            else if(_operator == CompareOperator.GreaterThan) return ((IComparable)value1).CompareTo((IComparable)value2) > 0;
            else if(_operator == CompareOperator.LessThanOrEqualTo) return ((IComparable)value1).CompareTo((IComparable)value2) <= 0;
            else if(_operator == CompareOperator.GreaterThanOrEqualTo) return ((IComparable)value1).CompareTo((IComparable)value2) >= 0;

            return false;
        }

        public string GetSummary(CommandBase parentCommand) {
            SharedStringBuilder.Append(_variableKey.GetSummary(parentCommand));
            SharedStringBuilder.Append("  ");
            SharedStringBuilder.Append(_operator.GetOperatorString());
            SharedStringBuilder.Append("  ");
            SharedStringBuilder.Append(_valueOrVariableKey.GetSummary(parentCommand));
            return SharedStringBuilder.Output();
        }

        public string Validate(CommandBase parentCommand, string label = null) {
            if(string.IsNullOrEmpty(label)) label = nameof(Condition);
            label += " : ";
            SharedStringBuilder.AppendAsNewLine(_variableKey.Validate(parentCommand, label: label + nameof(VariableKey)));
            SharedStringBuilder.AppendAsNewLine(_valueOrVariableKey.Validate(parentCommand, label: label + nameof(ValueOrVariableKey)));
            return SharedStringBuilder.Output();
        }
    }
}