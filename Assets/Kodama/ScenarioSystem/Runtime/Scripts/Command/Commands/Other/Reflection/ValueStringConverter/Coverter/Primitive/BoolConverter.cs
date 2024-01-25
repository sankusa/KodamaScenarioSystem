using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    internal class BoolConverter : ValueStringConverterBase {
        public override Type Type => typeof(bool);
        public override string InitialString => "false";

        public override string ValueToString(object value) {
            return ((bool)value).ToString();
        }

        public override object StringToValue(string valueString) {
            return bool.Parse(valueString);
        }

#if UNITY_EDITOR
        public override  string DrawField(Rect rect, string label, string valueString) {
            return ValueToString(EditorGUI.Toggle(rect, label, (bool)StringToValue(valueString)));
        }
#endif
    }
}