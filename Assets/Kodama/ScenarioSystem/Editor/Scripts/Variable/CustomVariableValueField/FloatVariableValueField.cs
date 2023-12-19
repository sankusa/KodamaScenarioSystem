using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(float))]
    public class FloatVariableValueField : VariableValueField<float> {
        protected override float Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as FloatVariable;
            return EditorGUI.FloatField(rect, variable.Value);
        }
    }
}