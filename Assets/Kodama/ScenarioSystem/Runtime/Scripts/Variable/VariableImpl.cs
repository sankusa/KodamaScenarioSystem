using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    // Primitive
    [Serializable]
    public class BoolVariable : Variable<bool> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate;
        }
    }

    [Serializable]
    public class DoubleVariable : Variable<double> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide
                || assignOperator == AssignOperator.Remind;
        }

        public override void Negate(double value) => _value = -value;
        public override void Add(double value) => _value += value;
        public override void Subtract(double value) => _value -= value;
        public override void Multiply(double value) => _value *= value;
        public override void Divide(double value) => _value /= value;
        public override void Remind(double value) => _value %= value;
    }

    [Serializable]
    public class FloatVariable : Variable<float> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide
                || assignOperator == AssignOperator.Remind;
        }

        public override void Negate(float value) => _value = -value;
        public override void Add(float value) => _value += value;
        public override void Subtract(float value) => _value -= value;
        public override void Multiply(float value) => _value *= value;
        public override void Divide(float value) => _value /= value;
        public override void Remind(float value) => _value %= value;
    }

    [Serializable]
    public class IntVariable : Variable<int> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide
                || assignOperator == AssignOperator.Remind;
        }

        public override void Negate(int value) => _value = -value;
        public override void Add(int value) => _value += value;
        public override void Subtract(int value) => _value -= value;
        public override void Multiply(int value) => _value *= value;
        public override void Divide(int value) => _value /= value;
        public override void Remind(int value) => _value %= value;
    }

    [Serializable]
    public class LongVariable : Variable<long> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide
                || assignOperator == AssignOperator.Remind;
        }

        public override void Negate(long value) => _value = -value;
        public override void Add(long value) => _value += value;
        public override void Subtract(long value) => _value -= value;
        public override void Multiply(long value) => _value *= value;
        public override void Divide(long value) => _value /= value;
        public override void Remind(long value) => _value %= value;
    }

    [Serializable]
    public class StringVariable : Variable<string> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Add;
        }

        public override void Add(string value) => _value += value;
    }

    [Serializable]
    public class ByteVariable : Variable<byte> {}

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
    public class Vector2Variable : Variable<Vector2> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide;
        }

        public override void Negate(Vector2 value) => _value = -value;
        public override void Add(Vector2 value) => _value += value;
        public override void Subtract(Vector2 value) => _value -= value;
        public override void Multiply(Vector2 value) => _value *= value;
        public override void Divide(Vector2 value) => _value /= value;
    }

    [Serializable]
    public class Vector3Variable : Variable<Vector3> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide;
        }

        public override void Negate(Vector3 value) => _value = -value;
        public override void Add(Vector3 value) => _value += value;
        public override void Subtract(Vector3 value) => _value -= value;
        public override void Multiply(Vector3 value) => _value.Scale(value);
        public override void Divide(Vector3 value) => _value.Scale(new Vector3(1 / value.x, 1 / value.y, 1 / value.z));
    }

    [Serializable]
    public class Vector4Variable : Variable<Vector4> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Negate
                || assignOperator == AssignOperator.Add
                || assignOperator == AssignOperator.Subtract
                || assignOperator == AssignOperator.Multiply
                || assignOperator == AssignOperator.Divide;
        }

        public override void Negate(Vector4 value) => _value = -value;
        public override void Add(Vector4 value) => _value += value;
        public override void Subtract(Vector4 value) => _value -= value;
        public override void Multiply(Vector4 value) => _value.Scale(value);
        public override void Divide(Vector4 value) => _value.Scale(new Vector4(1 / value.x, 1 / value.y, 1 / value.z, 1 / value.w));
    }

    // UniTask
    [Serializable]
    public class UniTaskVariable : Variable<UniTask> {}
}