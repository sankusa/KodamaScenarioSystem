using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    internal class Vector2Converter : ValueStringConverterBase {
        public override Type Type => typeof(Vector2);
        public override string InitialString => Vector2.zero.ToString();

        public override string ValueToString(object value) {
            Vector2 vector = (Vector2)value;
            return vector.x.ToString() + "," + vector.y.ToString();
        }

        public override object StringToValue(string valueString) {
            string[] values = valueString.Split(',');
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }

#if UNITY_EDITOR
        public override  string DrawField(Rect rect, string label, string valueString) {
            return ValueToString(EditorGUI.Vector2Field(rect, label, (Vector2)StringToValue(valueString)));
        }
#endif
    }
}