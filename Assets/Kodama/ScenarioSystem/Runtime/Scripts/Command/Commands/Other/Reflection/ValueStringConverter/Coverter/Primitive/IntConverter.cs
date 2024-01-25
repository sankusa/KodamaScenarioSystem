using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    internal class IntConverter : ValueStringConverterBase {
        public override Type Type => typeof(int);
        public override string InitialString => "0";

        public override string ValueToString(object value) {
            return ((int)value).ToString();
        }

        public override object StringToValue(string valueString) {
            return int.Parse(valueString);
        }

#if UNITY_EDITOR
        public override  string DrawField(Rect rect, string label, string valueString) {
            return ValueToString(EditorGUI.IntField(rect, label, (int)StringToValue(valueString)));
        }
#endif
    }
}