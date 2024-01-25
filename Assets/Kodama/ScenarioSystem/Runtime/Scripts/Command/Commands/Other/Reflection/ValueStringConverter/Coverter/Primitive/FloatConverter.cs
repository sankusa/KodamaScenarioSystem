using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    internal class FloatConverter : ValueStringConverterBase {
        public override Type Type => typeof(float);
        public override string InitialString => "0";

        public override string ValueToString(object value) {
            return ((float)value).ToString();
        }

        public override object StringToValue(string valueString) {
            return float.Parse(valueString);
        }

#if UNITY_EDITOR
        public override  string DrawField(Rect rect, string label, string valueString) {
            return ValueToString(EditorGUI.FloatField(rect, label, (float)StringToValue(valueString)));
        }
#endif
    }
}