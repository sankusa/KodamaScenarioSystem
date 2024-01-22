using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public static class SharedStringBuilder {
        private static StringBuilder _instance;
        private static StringBuilder Instance {
            get {
                if(_instance == null) {
                    _instance = new StringBuilder();
                }
                return _instance;
            }
        }

        public static void Append(string value) {
            if(string.IsNullOrEmpty(value)) return;
            Instance.Append(value);
        }

        public static void Append(char[] value) {
            if(value == null || value.Length == 0) return;
            Instance.Append(value);
        }

        public static void AppendAsNewLine(string value) {
            if(string.IsNullOrEmpty(value)) return;
            if(Instance.Length > 0) Instance.Append('\n');
            Instance.Append(value);
        }

        public static void AppendAsNewLine(char[] value) {
            if(value == null || value.Length == 0) return;
            if(Instance.Length > 0) Instance.Append('\n');
            Instance.Append(value);
        }

        public static string Output() {
            string ret = Instance.ToString();
            Instance.Clear();
            return ret;
        }
    }
}