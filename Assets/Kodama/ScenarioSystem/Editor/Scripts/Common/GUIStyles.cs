using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Gui;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class GUIStyles {
        public static GUIStyle SummaryLabel {get;}
        public static GUIStyle CenteredLabel {get;}
        public static GUIStyle ScenarioCaption {get;}
        public static GUIStyle ScenarioNameTextField {get;}
        public static GUIStyle BoldTextField {get;}
        public static GUIStyle LeanGroupBox {get;}
        public static GUIStyle TitleBar {get;}
        public static GUIStyle BorderedButton {get;}
        public static GUIStyle ClearButton {get;}
        public static GUIStyle CommandListElementStyle {get;}
        public static GUIStyle AsyncCommandSettingBox {get;}

        static GUIStyles() {
            SummaryLabel = new GUIStyle(EditorStyles.label) {
                richText = true,
                fontSize = 11,
            };
            CenteredLabel = new GUIStyle(EditorStyles.label) {
                alignment = TextAnchor.MiddleCenter,
            };
            ScenarioCaption = new GUIStyle(GUI.skin.box) {
                richText = true,
                margin = new RectOffset(),
            };
            ScenarioNameTextField = new GUIStyle(EditorStyles.textField) {
                richText = true,
                fontStyle = FontStyle.Bold,
                fixedHeight = 24,
                fontSize = 18,
            };
            BoldTextField = new GUIStyle(EditorStyles.textField) {
                richText = true,
                fontStyle = FontStyle.Bold,
            };
            LeanGroupBox = new GUIStyle("GroupBox") {
                margin = new RectOffset(),
                padding = new RectOffset(),
            };
            TitleBar = new GUIStyle("TimeAreaToolbar") {
                margin = new RectOffset(),
                padding = new RectOffset()
            };
            BorderedButton = "AppToolbarButtonLeft";
            ClearButton = "RL FooterButton";
            CommandListElementStyle = new GUIStyle("GroupBox") {
                margin = new RectOffset(0, 0, 0, 0)
            };
            AsyncCommandSettingBox = new GUIStyle("flow node 0");
        }
    }
}