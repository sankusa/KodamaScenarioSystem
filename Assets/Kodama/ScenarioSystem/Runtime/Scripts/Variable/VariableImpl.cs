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

        public override void Negate(IVariableValueHolder<bool> value) => _value = !value.Value;
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

        public override void Negate(IVariableValueHolder<double> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<double> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<double> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<double> value) => _value *= value.Value;
        public override void Divide(IVariableValueHolder<double> value) => _value /= value.Value;
        public override void Remind(IVariableValueHolder<double> value) => _value %= value.Value;
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

        public override void Negate(IVariableValueHolder<float> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<float> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<float> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<float> value) => _value *= value.Value;
        public override void Divide(IVariableValueHolder<float> value) => _value /= value.Value;
        public override void Remind(IVariableValueHolder<float> value) => _value %= value.Value;
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

        public override void Negate(IVariableValueHolder<int> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<int> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<int> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<int> value) => _value *= value.Value;
        public override void Divide(IVariableValueHolder<int> value) => _value /= value.Value;
        public override void Remind(IVariableValueHolder<int> value) => _value %= value.Value;
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

        public override void Negate(IVariableValueHolder<long> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<long> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<long> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<long> value) => _value *= value.Value;
        public override void Divide(IVariableValueHolder<long> value) => _value /= value.Value;
        public override void Remind(IVariableValueHolder<long> value) => _value %= value.Value;
    }

    [Serializable]
    public class StringVariable : Variable<string> {
        public override bool IsValidArthmeticOperator(AssignOperator assignOperator) {
            return assignOperator == AssignOperator.Add;
        }

        public override void Add(IVariableValueHolder<string> value) => _value += value.Value;
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

        public override void Negate(IVariableValueHolder<Vector2> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<Vector2> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<Vector2> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<Vector2> value) => _value *= value.Value;
        public override void Divide(IVariableValueHolder<Vector2> value) => _value /= value.Value;
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

        public override void Negate(IVariableValueHolder<Vector3> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<Vector3> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<Vector3> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<Vector3> value) => _value.Scale(value.Value);
        public override void Divide(IVariableValueHolder<Vector3> value) => _value.Scale(new Vector3(1 / value.Value.x, 1 / value.Value.y, 1 / value.Value.z));
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

        public override void Negate(IVariableValueHolder<Vector4> value) => _value = -value.Value;
        public override void Add(IVariableValueHolder<Vector4> value) => _value += value.Value;
        public override void Subtract(IVariableValueHolder<Vector4> value) => _value -= value.Value;
        public override void Multiply(IVariableValueHolder<Vector4> value) => _value.Scale(value.Value);
        public override void Divide(IVariableValueHolder<Vector4> value) => _value.Scale(new Vector4(1 / value.Value.x, 1 / value.Value.y, 1 / value.Value.z, 1 / value.Value.w));
    }

    // UniTask
    [Serializable]
    public class UniTaskVariable : Variable<UniTask> {}
}