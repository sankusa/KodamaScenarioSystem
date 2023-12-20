using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolVariableName : VariableName<bool> {}

    [Serializable]
    public class ByteVariableName : VariableName<byte> {}

    [Serializable]
    public class DoubleVariableName : VariableName<double> {}

    [Serializable]
    public class FloatVariableName : VariableName<float> {}

    [Serializable]
    public class IntVariableName : VariableName<int> {}

    [Serializable]
    public class LongVariableName : VariableName<long> {}

    [Serializable]
    public class StringVariableName : VariableName<string> {}

    // Unity
    [Serializable]
    public class AnimationCurveVariableName : VariableName<AnimationCurve> {}

    [Serializable]
    public class BoundsVariableName : VariableName<Bounds> {}

    [Serializable]
    public class ColorVariableName : VariableName<Color> {}

    [Serializable]
    public class QuaternionVariableName : VariableName<Quaternion> {}

    [Serializable]
    public class RectVariableName : VariableName<Rect> {}

    [Serializable]
    public class Vector2VariableName : VariableName<Vector2> {}

    [Serializable]
    public class Vector3VariableName : VariableName<Vector3> {}

    [Serializable]
    public class Vector4VariableName : VariableName<Vector4> {}

    // UniTask
    [Serializable]
    public class UniTaskVariableName : VariableName<UniTask> {}
}