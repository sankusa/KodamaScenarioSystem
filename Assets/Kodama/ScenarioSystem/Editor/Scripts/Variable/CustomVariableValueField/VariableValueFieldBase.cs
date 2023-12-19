using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Kodama.ScenarioSystem;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public abstract class VariableValueFieldBase {
        public virtual void Draw(Rect rect, Scenario scenario, VariableBase variable) {}
        public virtual float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}