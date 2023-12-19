using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(bool))]
    public class BoolVariableValueField : VariableValueField<bool> {
        protected override bool Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as BoolVariable;
            return EditorGUI.Toggle(rect, variable.Value);
        }
    }
}