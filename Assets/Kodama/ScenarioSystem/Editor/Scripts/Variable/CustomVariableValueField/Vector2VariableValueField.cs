using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Vector2))]
    public class Vector2VariableValueField : VariableValueField<Vector2> {
        protected override Vector2 Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as Vector2Variable;
            return EditorGUI.Vector2Field(rect, GUIContent.none, variable.Value);
        }
    }
}