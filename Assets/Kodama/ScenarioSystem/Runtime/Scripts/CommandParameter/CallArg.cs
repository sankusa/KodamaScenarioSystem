using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class CallArg {
        [SerializeField] protected string _variableId;
        public string VariableId => _variableId;

        public abstract Type TargetType {get;}

        public abstract string GetSummary(CommandBase parentCommand);
        public abstract string Validate(CommandBase parentCommand, string label = null);
    }

    [Serializable]
    public abstract class CallArg<T> : CallArg {
        public override Type TargetType => typeof(T);

        [SerializeField] protected T _value;
        public T Value => _value;

        public override string GetSummary(CommandBase parentCommand) {
            if(_variableId == "") {
                return Labels.Label_Empty_Red;
            }

            Variable<T> targetVariable = parentCommand.GetAvailableVariableDefines<T>().FirstOrDefault(x => x.Id == _variableId);

            if(targetVariable == null) return Labels.Label_MissingReference_Red;

            SharedStringBuilder.Append("<i>[");
            SharedStringBuilder.Append(targetVariable.Name);
            SharedStringBuilder.Append("]</i> = ");
            SharedStringBuilder.Append(_value.ToString());
            return SharedStringBuilder.Output();
        }

        public override string Validate(CommandBase parentCommand, string label = null) {
            if(string.IsNullOrEmpty(label)) label = nameof(VariableKey);
            if(string.IsNullOrEmpty(_variableId)) {
                return label + " is empty";
            } else if(parentCommand.GetAvailableVariableDefines<T>().FirstOrDefault(x => x.Id == _variableId) == null) {
                return label + " missing reference";
            }
            return "";
        }
    }
}