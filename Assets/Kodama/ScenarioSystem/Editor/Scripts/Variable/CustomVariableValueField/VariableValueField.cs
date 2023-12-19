using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public abstract class VariableValueField<T> : VariableValueFieldBase {
        public sealed override void Draw(Rect rect, Scenario scenario, VariableBase variableBase) {
            EditorGUI.BeginChangeCheck();
            T value = Field(rect, variableBase);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(scenario, $"{TypeNameUtil.ConvertToPrimitiveTypeName(value.GetType().Name)} Variable Value Change");
                Variable<T> variable = variableBase as Variable<T>;
                variable.Value = value;
                EditorUtility.SetDirty(scenario);
            }
        }

        protected abstract T Field(Rect rect, VariableBase variableBase);
    }
}