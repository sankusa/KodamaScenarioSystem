using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public abstract class ValueStringConverterBase {
        // 対象の型
        public abstract Type Type {get;}
        // 初期化用文字列
        public abstract string InitialString {get;}
        // 変換関数(value → string)
        public abstract string ValueToString(object value);
        // 変換関数(string → value)
        public abstract object StringToValue(string valueString);
#if UNITY_EDITOR
        public abstract string DrawField(Rect rect, string label, string valueString);
#endif
    }
}