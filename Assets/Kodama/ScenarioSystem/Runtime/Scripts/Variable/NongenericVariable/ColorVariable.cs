using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ColorVariable : Variable<Color> {
        public ColorVariable() {
            Value = Color.white;
        }
    }
}