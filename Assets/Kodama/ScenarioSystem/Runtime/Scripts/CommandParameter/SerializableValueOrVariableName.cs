using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolValueOrVariableName : ValueOrVariableName<bool> {}

    [Serializable]
    public class ByteValueOrVariableName : ValueOrVariableName<byte> {}

    [Serializable]
    public class DoubleValueOrVariableName : ValueOrVariableName<double> {}

    [Serializable]
    public class FloatValueOrVariableName : ValueOrVariableName<float> {}

    [Serializable]
    public class IntValueOrVariableName : ValueOrVariableName<int> {}

    [Serializable]
    public class LongValueOrVariableName : ValueOrVariableName<long> {}

    [Serializable]
    public class StringValueOrVariableName : ValueOrVariableName<string> {}

    // Unity
    [Serializable]
    public class AnimationCurveValueOrVariableName : ValueOrVariableName<AnimationCurve> {}

    [Serializable]
    public class BoundsValueOrVariableName : ValueOrVariableName<Bounds> {}

    [Serializable]
    public class ColorValueOrVariableName : ValueOrVariableName<Color> {}

    [Serializable]
    public class QuaternionValueOrVariableName : ValueOrVariableName<Quaternion> {}

    [Serializable]
    public class RectValueOrVariableName : ValueOrVariableName<Rect> {}

    [Serializable]
    public class Vector2ValueOrVariableName : ValueOrVariableName<Vector2> {}

    [Serializable]
    public class Vector3ValueOrVariableName : ValueOrVariableName<Vector3> {}

    [Serializable]
    public class Vector4ValueOrVariableName : ValueOrVariableName<Vector4> {}

    [Serializable]
    public class UniTaskValueOrVariableName : ValueOrVariableName<UniTask> {}
}