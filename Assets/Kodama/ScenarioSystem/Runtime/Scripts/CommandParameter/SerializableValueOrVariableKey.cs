using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolValueOrVariableKey : ValueOrVariableKey<bool> {}

    [Serializable]
    public class ByteValueOrVariableKey : ValueOrVariableKey<byte> {}

    [Serializable]
    public class DoubleValueOrVariableKey : ValueOrVariableKey<double> {}

    [Serializable]
    public class FloatValueOrVariableKey : ValueOrVariableKey<float> {}

    [Serializable]
    public class IntValueOrVariableKey : ValueOrVariableKey<int> {}

    [Serializable]
    public class LongValueOrVariableKey : ValueOrVariableKey<long> {}

    [Serializable]
    public class StringValueOrVariableKey : ValueOrVariableKey<string> {}

    // Unity
    [Serializable]
    public class AnimationCurveValueOrVariableKey : ValueOrVariableKey<AnimationCurve> {}

    [Serializable]
    public class BoundsValueOrVariableKey : ValueOrVariableKey<Bounds> {}

    [Serializable]
    public class ColorValueOrVariableKey : ValueOrVariableKey<Color> {}

    [Serializable]
    public class QuaternionValueOrVariableKey : ValueOrVariableKey<Quaternion> {}

    [Serializable]
    public class RectValueOrVariableKey : ValueOrVariableKey<Rect> {}

    [Serializable]
    public class Vector2ValueOrVariableKey : ValueOrVariableKey<Vector2> {}

    [Serializable]
    public class Vector3ValueOrVariableKey : ValueOrVariableKey<Vector3> {}

    [Serializable]
    public class Vector4ValueOrVariableKey : ValueOrVariableKey<Vector4> {}

    // UniTask
    [Serializable]
    public class UniTaskValueOrVariableKey : ValueOrVariableKey<UniTask> {}
}