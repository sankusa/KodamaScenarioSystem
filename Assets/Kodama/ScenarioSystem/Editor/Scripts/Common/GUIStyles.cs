using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class GUIStyles {
        public static GUIStyle SummaryLabel {get;}
        public static GUIStyle LeanGroupBox {get;}
        public static GUIStyle TitleBar {get;}
        public static GUIStyle BorderedButton {get;}
        public static GUIStyle ClearButton {get;}

        static GUIStyles() {
            SummaryLabel = new GUIStyle(EditorStyles.label) {
                richText = true,
                wordWrap = true
            };
            LeanGroupBox = new GUIStyle("GroupBox") {
                margin = new RectOffset(),
                padding = new RectOffset()
            };
            TitleBar = new GUIStyle("TimeAreaToolbar") {
                margin = new RectOffset(),
                padding = new RectOffset()
            };
            BorderedButton = new GUIStyle("AppToolbarButtonLeft");
            ClearButton = "RL FooterButton";
        }
    }
}