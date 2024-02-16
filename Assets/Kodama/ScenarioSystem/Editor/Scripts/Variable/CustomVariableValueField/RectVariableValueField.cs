using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(Rect))]
    public class RectVariableValueField : VariableValueField<Rect> {
        protected override Rect Field(Rect rect, IVariableValueHolder<Rect> variableValueHolder) {
            return EditorGUI.RectField(rect, variableValueHolder.Value);
        }

        public override float GetHeight() {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}