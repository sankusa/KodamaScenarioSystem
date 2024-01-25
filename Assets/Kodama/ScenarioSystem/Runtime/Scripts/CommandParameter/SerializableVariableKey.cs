using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolVariableKey : VariableKey<bool> {}

    [Serializable]
    public class DoubleVariableKey : VariableKey<double> {}

    [Serializable]
    public class FloatVariableKey : VariableKey<float> {}

    [Serializable]
    public class IntVariableKey : VariableKey<int> {}

    [Serializable]
    public class LongVariableKey : VariableKey<long> {}

    [Serializable]
    public class StringVariableKey : VariableKey<string> {}

    // Unity
    [Serializable]
    public class AnimationCurveVariableKey : VariableKey<AnimationCurve> {}

    [Serializable]
    public class BoundsVariableKey : VariableKey<Bounds> {}

    [Serializable]
    public class ColorVariableKey : VariableKey<Color> {}

    [Serializable]
    public class QuaternionVariableKey : VariableKey<Quaternion> {}

    [Serializable]
    public class RectVariableKey : VariableKey<Rect> {}

    [Serializable]
    public class Vector2VariableKey : VariableKey<Vector2> {}

    [Serializable]
    public class Vector3VariableKey : VariableKey<Vector3> {}

    [Serializable]
    public class Vector4VariableKey : VariableKey<Vector4> {}

    // UniTask
    [Serializable]
    public class UniTaskVariableKey : VariableKey<UniTask> {}
}