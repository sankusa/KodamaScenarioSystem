using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(double))]
    public class DoubleVariableValueField : VariableValueField<double> {
        protected override double Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as DoubleVariable;
            return EditorGUI.DoubleField(rect, variable.Value);
        }
    }
}