using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableKey {
        [SerializeField] protected string _id;
        public string Id => _id;

        public abstract Type TargetType {get;}

        public bool IsEmpty() => string.IsNullOrEmpty(_id);

        public abstract string GetSummary(CommandBase parentCommand, bool warnEmpty = true);
        public abstract string Validate(CommandBase parentCommand, bool warnEmpty = true, string name = null);
    }

    [Serializable]
    /// <typeparam name="T">型に合わせた入力フィールドを生成するため、エディタ側で型引数を使用する</typeparam>
    public class VariableKey<T> : VariableKey {
        public override Type TargetType => typeof(T);

        public override string GetSummary(CommandBase parentCommand, bool warnEmpty = true) {
            if(_id == "") {
                if(warnEmpty) {
                    return Labels.Label_Empty_Red;
                }
                else {
                    return Labels.Label_Empty;
                }
            }

            Variable<T> targetVariable = parentCommand.GetAvailableVariableDefines<T>().FirstOrDefault(x => x.Id == _id);

            if(targetVariable == null) return Labels.Label_MissingReference_Red;

            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append("<i>[");
            sb.Append(targetVariable.Name);
            sb.Append("]</i>");
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public override string Validate(CommandBase parentCommand, bool warnEmpty = true, string name = null) {
            if(string.IsNullOrEmpty(name)) name = nameof(VariableKey);
            if(string.IsNullOrEmpty(_id)) {
                if(warnEmpty) return name + " is empty";
            } else if(parentCommand.GetAvailableVariableDefines<T>().FirstOrDefault(x => x.Id == _id) == null) {
                return name + " missing reference";
            }
            return "";
        }
    }
}