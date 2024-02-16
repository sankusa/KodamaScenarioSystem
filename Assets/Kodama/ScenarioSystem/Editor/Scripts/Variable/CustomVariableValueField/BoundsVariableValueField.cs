using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Bounds))]
    public class BoundsVariableValueField : VariableValueField<Bounds> {
        protected override Bounds Field(Rect rect, IVariableValueHolder<Bounds> variableValueHolder) {
            return EditorGUI.BoundsField(rect, variableValueHolder.Value);
        }

        public override float GetHeight() {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}