using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableBase {
        public abstract string Name {get;set;}
        internal abstract VariableBase Copy();
    }
}