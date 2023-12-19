using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Color))]
    public class ColorVariableValueField : VariableValueField<Color> {
        protected override Color Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as ColorVariable;
            return EditorGUI.ColorField(rect, variable.Value);
        }
    }
}