using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Vector4))]
    public class Vector4VariableValueField : VariableValueField<Vector4> {
        protected override Vector4 Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as Vector4Variable;
            return EditorGUI.Vector4Field(rect, GUIContent.none, variable.Value);
        }
    }
}