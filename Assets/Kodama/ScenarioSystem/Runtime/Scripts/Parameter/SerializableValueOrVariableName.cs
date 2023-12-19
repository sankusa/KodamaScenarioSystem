using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class IntValueOrVariableName : ValueOrVariableName<int> {}
    [Serializable]
    public class StringValueOrVariableName : ValueOrVariableName<string> {}
    [Serializable]
    public class BoolValueOrVariableName : ValueOrVariableName<bool> {}
    [Serializable]
    public class Vector2ValueOrVariableName : ValueOrVariableName<Vector2> {}
}