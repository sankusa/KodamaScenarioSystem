using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Bounds))]
    public class BoundsVariableValueField : VariableValueField<Bounds> {
        protected override Bounds Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as BoundsVariable;
            return EditorGUI.BoundsField(rect, variable.Value);
        }

        public override float GetHeight() {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}