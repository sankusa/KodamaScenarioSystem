using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Vector3))]
    public class Vector3VariableValueField : VariableValueField<Vector3> {
        protected override Vector3 Field(Rect rect, IVariableValueHolder<Vector3> variableValueHolder) {
            return EditorGUI.Vector3Field(rect, GUIContent.none, variableValueHolder.Value);
        }
    }
}