using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(string))]
    public class StringVariableValueField : VariableValueField<string> {
        protected override string Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as StringVariable;
            return EditorGUI.TextField(rect, variable.Value);
        }
    }
}