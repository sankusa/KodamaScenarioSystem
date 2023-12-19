using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public abstract class VariableBase {
        public abstract string Name {get;set;}
        public abstract object GetValueAsObject();
        public abstract void SetValueAsObject(object obj);
        public abstract Type TargetType {get;}
        internal abstract VariableBase Copy();
    }
}