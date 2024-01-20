using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableName {
        [SerializeField] private string _name;
        public string Name => _name;

        public abstract Type TargetType {get;}
        public virtual string GetSummary(bool warnEmpty = true) {
            if(_name == "") {
                if(warnEmpty) {
                    return Labels.Label_Empty_Red;
                }
                else {
                    return Labels.Label_Empty;
                }
            }

            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append("<i>[");
            sb.Append(_name);
            sb.Append("]</i>");
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public abstract string Validate(CommandBase parentCommand, bool warn = true);
    }

    [Serializable]
    /// <typeparam name="T">型に合わせた入力フィールドを生成するため、エディタ側で型引数を使用する</typeparam>
    public class VariableName<T> : VariableName {
        public override Type TargetType => typeof(T);

        public override string Validate(CommandBase parentCommand, bool warn = true) {
            if(warn == false) return "";
            
            ScenarioPage page = parentCommand.ParentPage;
            Scenario scenario= page.ParentScenario;
            if(string.IsNullOrEmpty(Name)) {
                return "Variable name is empty";
            } else if(scenario.Variables.OfType<Variable<T>>().FirstOrDefault(x => x.Name == Name) == null) {
                return "Variable not found";
            }
            return "";
        }
    }
}