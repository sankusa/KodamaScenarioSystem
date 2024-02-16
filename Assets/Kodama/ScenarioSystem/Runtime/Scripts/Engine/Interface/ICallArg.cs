using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface ICallArg : IVariableValueHolder {
        string VariableId {get;}
        Type TargetType {get;}
    }

    public interface ICallArg<T> : ICallArg, IVariableValueHolder<T> {}
}