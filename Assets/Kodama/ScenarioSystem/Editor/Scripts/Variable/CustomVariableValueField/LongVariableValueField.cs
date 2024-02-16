using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(long))]
    public class LongVariableValueField : VariableValueField<long> {
        protected override long Field(Rect rect, IVariableValueHolder<long> variableValueHolder) {
            return EditorGUI.LongField(rect, variableValueHolder.Value);
        }
    }
}