using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class ContentColorScope : GUI.Scope {
        private Color defaultColor;

        public ContentColorScope(Color overwriteColor) {
            defaultColor = GUI.backgroundColor;
            GUI.contentColor = overwriteColor;
        }

        protected override void CloseScope() {
            GUI.contentColor = defaultColor;
        }
    }
}
