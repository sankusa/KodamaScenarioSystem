using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolVariable : Variable<bool> {}

    [Serializable]
    public class DoubleVariable : Variable<double> {}

    [Serializable]
    public class FloatVariable : Variable<float> {}

    [Serializable]
    public class IntVariable : Variable<int> {}

    [Serializable]
    public class LongVariable : Variable<long> {}

    [Serializable]
    public class StringVariable : Variable<string> {}

    // Unity
    [Serializable]
    public class AnimationCurveVariable : Variable<AnimationCurve> {}

    [Serializable]
    public class BoundsVariable : Variable<Bounds> {}

    [Serializable]
    public class ColorVariable : Variable<Color> {
        public ColorVariable() {
            Value = Color.white;
        }
    }

    [Serializable]
    public class QuaternionVariable : Variable<Quaternion> {}

    [Serializable]
    public class RectVariable : Variable<Rect> {}

    [Serializable]
    public class Vector2Variable : Variable<Vector2> {}

    [Serializable]
    public class Vector3Variable : Variable<Vector3> {}

    [Serializable]
    public class Vector4Variable : Variable<Vector4> {}

    // UniTask
    [Serializable]
    public class UniTaskVariable : Variable<UniTask> {}
}