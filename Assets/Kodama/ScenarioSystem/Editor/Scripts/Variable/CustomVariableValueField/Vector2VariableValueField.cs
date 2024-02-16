using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Vector2))]
    public class Vector2VariableValueField : VariableValueField<Vector2> {
        protected override Vector2 Field(Rect rect, IVariableValueHolder<Vector2> variableValueHolder) {
            return EditorGUI.Vector2Field(rect, GUIContent.none, variableValueHolder.Value);
        }
    }
}