using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class GUIStyles {
        private static GUIStyle _summaryLabel;
        public static GUIStyle SummaryLabel {
            get {
                if(_summaryLabel == null) {
                    _summaryLabel = new GUIStyle(EditorStyles.label) {
                        richText = true,
                        wordWrap = true
                    };
                }
                return _summaryLabel;
            }
        }

        private static GUIStyle _leanGroupBox;
        public static GUIStyle LeanGroupBox {
            get {
                if(_leanGroupBox == null) {
                    _leanGroupBox = new GUIStyle("GroupBox") {
                        margin = new RectOffset(),
                        padding = new RectOffset()
                    };
                }
                return _leanGroupBox;
            }
        }

        private static GUIStyle _titleBar;
        public static GUIStyle TitleBar {
            get {
                if(_titleBar == null) {
                    _titleBar = new GUIStyle("TimeAreaToolbar") {
                        margin = new RectOffset(),
                        padding = new RectOffset()
                    };
                }
                return _titleBar;
            }
        }

        private static GUIStyle _borderedButton;
        public static GUIStyle BorderedButton {
            get {
                _borderedButton ??= new GUIStyle("AppToolbarButtonLeft");
                return _borderedButton;
            }
        }
    }
}