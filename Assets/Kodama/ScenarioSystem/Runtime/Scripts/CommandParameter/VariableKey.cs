using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableKey : IVariableKey {
        [SerializeField] protected string _id;
        public string Id => _id;

        public abstract Type TargetType {get;}

        public bool IsEmpty() => string.IsNullOrEmpty(_id);

        public abstract string GetSummary(CommandBase parentCommand, bool warnEmpty = true);
        public abstract string Validate(CommandBase parentCommand, bool warnEmpty = true, string label = null);
    }

    [Serializable]
    /// <typeparam name="T">型に合わせた入力フィールドを生成するため、エディタ側で型引数を使用する</typeparam>
    public class VariableKey<T> : VariableKey, IVariableKey<T> {
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

            SharedStringBuilder.Append("<i>[");
            SharedStringBuilder.Append(targetVariable.Name);
            SharedStringBuilder.Append("]</i>");
            return SharedStringBuilder.Output();
        }

        public override string Validate(CommandBase parentCommand, bool warnEmpty = true, string label = null) {
            if(string.IsNullOrEmpty(label)) label = nameof(VariableKey);
            if(string.IsNullOrEmpty(_id)) {
                if(warnEmpty) return label + " is empty";
            } else if(parentCommand.GetAvailableVariableDefines<T>().FirstOrDefault(x => x.Id == _id) == null) {
                return label + " missing reference";
            }
            return "";
        }
    }
}