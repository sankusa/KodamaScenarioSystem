using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public static class SharedStringBuilder {
        private static StringBuilder _instance;
        public static StringBuilder Instance {
            get {
                if(_instance == null) {
                    _instance = new StringBuilder();
                }
                return _instance;
            }
        }
    }
}