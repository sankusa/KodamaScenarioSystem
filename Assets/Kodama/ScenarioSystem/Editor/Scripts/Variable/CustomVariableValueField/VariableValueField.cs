using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public abstract class VariableValueField<T> : VariableValueFieldBase {
        public sealed override void Draw(Rect rect, ScriptableObject objectForUndo, IVariableValueHolder variableValueHolder) {
            IVariableValueHolder<T> genericVariableValueHolder = variableValueHolder as IVariableValueHolder<T>;
            EditorGUI.BeginChangeCheck();
            T value = Field(rect, genericVariableValueHolder);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(objectForUndo, $"{TypeNameUtil.ConvertToPrimitiveTypeName(value.GetType().Name)} Variable Value Change");
                genericVariableValueHolder.Value = value;
            }
        }

        protected abstract T Field(Rect rect, IVariableValueHolder<T> variableValueHolder);
    }
}