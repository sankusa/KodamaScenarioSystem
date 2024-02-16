using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(UniTask))]
    public class UniTaskVariableValueField : VariableValueField<UniTask> {
        protected override UniTask Field(Rect rect, IVariableValueHolder<UniTask> variableValueHolder) {
            EditorGUI.LabelField(rect, "Default");
            return default;
        }
    }
}