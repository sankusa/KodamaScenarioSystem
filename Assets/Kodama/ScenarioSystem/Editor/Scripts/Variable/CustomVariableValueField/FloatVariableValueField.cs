using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(float))]
    public class FloatVariableValueField : VariableValueField<float> {
        protected override float Field(Rect rect, IVariableValueHolder<float> variableValueHolder) {
            return EditorGUI.FloatField(rect, variableValueHolder.Value);
        }
    }
}