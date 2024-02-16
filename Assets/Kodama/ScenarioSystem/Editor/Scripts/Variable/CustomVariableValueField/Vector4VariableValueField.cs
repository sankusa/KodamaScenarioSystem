using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Vector4))]
    public class Vector4VariableValueField : VariableValueField<Vector4> {
        protected override Vector4 Field(Rect rect, IVariableValueHolder<Vector4> variableValueHolder) {
            return EditorGUI.Vector4Field(rect, GUIContent.none, variableValueHolder.Value);
        }
    }
}