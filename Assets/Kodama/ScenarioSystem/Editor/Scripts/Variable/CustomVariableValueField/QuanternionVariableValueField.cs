using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Quaternion))]
    public class QuaternionVariableValueField : VariableValueField<Quaternion> {
        protected override Quaternion Field(Rect rect, IVariableValueHolder<Quaternion> variableValueHolder) {
            Vector4 vector4 = EditorGUI.Vector4Field(
                rect, GUIContent.none,
                new Vector4(
                    variableValueHolder.Value.x,
                    variableValueHolder.Value.y,
                    variableValueHolder.Value.z,
                    variableValueHolder.Value.w
                )
            );
            return new Quaternion(vector4.x, vector4.y, vector4.z, vector4.w);
        }
    }
}