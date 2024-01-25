using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    internal class StringConverter : ValueStringConverterBase {
        public override Type Type => typeof(string);
        public override string InitialString => "";

        public override string ValueToString(object value) {
            return (string) value;
        }

        public override object StringToValue(string valueString) {
            return valueString;
        }

#if UNITY_EDITOR
        public override  string DrawField(Rect rect, string label, string valueString) {
            return EditorGUI.TextField(rect, label, valueString);
        }
#endif
    }
}