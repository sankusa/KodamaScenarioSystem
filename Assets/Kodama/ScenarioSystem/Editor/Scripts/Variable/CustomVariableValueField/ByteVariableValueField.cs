using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomVariableValueField(typeof(byte))]
    public class ByteVariableValueField : VariableValueField<byte> {
        protected override byte Field(Rect rect, VariableBase variableBase) {
            var variable = variableBase as ByteVariable;
            return (byte)Mathf.Clamp(EditorGUI.IntField(rect, variable.Value), 0, 255);
        }
    }
}