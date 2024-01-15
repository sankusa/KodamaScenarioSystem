using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class ColorScope : GUI.Scope {
        private Color defaultColor;

        public ColorScope(Color overwriteColor) {
            defaultColor = GUI.backgroundColor;
            GUI.color = overwriteColor;
        }

        protected override void CloseScope() {
            GUI.color = defaultColor;
        }
    }
}
