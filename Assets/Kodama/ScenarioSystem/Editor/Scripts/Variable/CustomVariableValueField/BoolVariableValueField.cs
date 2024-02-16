using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(bool))]
    public class BoolVariableValueField : VariableValueField<bool> {
        protected override bool Field(Rect rect, IVariableValueHolder<bool> variableValueHolder) {
            return EditorGUI.Toggle(rect, variableValueHolder.Value);
        }
    }
}