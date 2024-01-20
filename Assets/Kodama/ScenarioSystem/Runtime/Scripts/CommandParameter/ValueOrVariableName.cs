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
    public abstract class ValueOrVariableName {
        [SerializeField] protected string _variableName;
        public string VariableName => _variableName;

        public abstract object GetValueAsObject();

        public abstract Type TargetType {get;}
        public abstract string GetSummary();
        public abstract string Validate(CommandBase parentCommand);
    }
    [Serializable]
    public class ValueOrVariableName<T> : ValueOrVariableName {
        [SerializeField] private T _value;
        public T Value => _value;

        public override object GetValueAsObject() => _value;

        public override Type TargetType => typeof(T);
        public override string GetSummary() {
            if(string.IsNullOrEmpty(_variableName)) {
                return _value != null ? _value.ToString() : Labels.Label_Null;
            }

            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append("<i>[");
            sb.Append(_variableName);
            sb.Append("]</i>");
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public override string Validate(CommandBase parentCommand) {
            if(string.IsNullOrEmpty(_variableName)) return "";

            ScenarioPage page = parentCommand.ParentPage;
            Scenario scenario= page.ParentScenario;
            if(scenario.Variables.OfType<Variable<T>>().FirstOrDefault(x => x.Name == VariableName) == null) {
                return "Variable not found";
            }
            return "";
        }
    }
}