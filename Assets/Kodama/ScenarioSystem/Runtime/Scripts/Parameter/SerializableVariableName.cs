using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class IntVariableName : VariableName<int> {}
    [Serializable]
    public class StringVariableName : VariableName<string> {}
    [Serializable]
    public class BoolVariableName : VariableName<bool> {}
    [Serializable]
    public class Vector2VariableName : VariableName<Vector2> {}
}