using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class VariableSetter {
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
        [SerializeField] private AssignOperator _operator;
        public AssignOperator Operator {
            get => _operator;
            set => _operator = value;
        }

        public VariableSetter() {
            _variableKey = new IntVariableKey();
            _valueOrVariableKey = new IntValueOrVariableKey();
        }

        public void Set(IPagePlayProcess process) {
            VariableBase variable = process.FindVariable(_variableKey);
            object value;
            if(_valueOrVariableKey.HasKey()) {
                value = process.FindVariable(_valueOrVariableKey.VariableKey).GetValueAsObject();
            }
            else {
                value = _valueOrVariableKey.GetValueAsObject();
            }
            if(_operator == AssignOperator.Assign) variable.SetValueAsObject(value);
            else if(_operator == AssignOperator.Negate) variable.Negate(value);
            else if(_operator == AssignOperator.Add) variable.Add(value);
            else if(_operator == AssignOperator.Subtract) variable.Subtract(value);
            else if(_operator == AssignOperator.Multiply) variable.Multiply(value);
            else if(_operator == AssignOperator.Divide) variable.Divide(value);
            else if(_operator == AssignOperator.Remind) variable.Remind(value);
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