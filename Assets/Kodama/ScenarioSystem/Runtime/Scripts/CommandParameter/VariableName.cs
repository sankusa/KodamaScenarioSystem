using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableName {
        [SerializeField] private string _name;
        public string Name => _name;

        public abstract Type TargetType {get;}
        public virtual string GetSummary() => $"<i>{_name}</i>";
    }

    [Serializable]
    /// <typeparam name="T">型に合わせた入力フィールドを生成するため、エディタ側で型引数を使用する</typeparam>
    public class VariableName<T> : VariableName {
        public override Type TargetType => typeof(T);
    }
}