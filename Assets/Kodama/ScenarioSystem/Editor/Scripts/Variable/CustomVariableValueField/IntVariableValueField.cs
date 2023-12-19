using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(int))]
    public class IntVariableValueField : VariableValueField<int> {
        protected override int Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as IntVariable;
            return EditorGUI.IntField(rect, variable.Value);
        }
    }
}