using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Color))]
    public class ColorVariableValueField : VariableValueField<Color> {
        protected override Color Field(Rect rect, IVariableValueHolder<Color> variableValueHolder) {
            return EditorGUI.ColorField(rect, variableValueHolder.Value);
        }
    }
}