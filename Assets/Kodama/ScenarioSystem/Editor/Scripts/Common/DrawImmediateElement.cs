using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    /// <summary>
    /// Handles等の描画メソッドをImmediateRepaintから呼び出すことでVisualElementより前面に描画できる。
    /// </summary>
    public class DrawImmediateElement : ImmediateModeElement {
        public event Action OnRepaint;
        protected override void ImmediateRepaint() {
            OnRepaint?.Invoke();
        }
    }
}