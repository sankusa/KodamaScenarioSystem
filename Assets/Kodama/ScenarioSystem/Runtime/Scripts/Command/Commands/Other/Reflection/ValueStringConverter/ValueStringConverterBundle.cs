using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ValueStringConverterBundle {
        // ランタイムのみstatic領域に配置
        // エディタでは都度インスタンスを作成
        private static ValueStringConverterBundle _instance;
        public static ValueStringConverterBundle Instance {
            get {
                if(_instance == null) {
                    _instance = new ValueStringConverterBundle();
                }
                return _instance;
            }
        }

        private ValueStringConverterBase[] _converters;
        public ValueStringConverterBundle() {
            _converters = Assembly.GetAssembly(typeof(ValueStringConverterBundle))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ValueStringConverterBase)))
                .OrderBy(x => x.IsPrimitive)
                .Select(x => Activator.CreateInstance(x) as ValueStringConverterBase)
                .ToArray();
        }

        public ValueStringConverterBase FindConverter(Type type) {
            return _converters.FirstOrDefault(x => x.Type == type);
        }

        public bool IsConvertibleType(Type type) {
            return _converters.Any(x => x.Type == type);
        }
    }
}