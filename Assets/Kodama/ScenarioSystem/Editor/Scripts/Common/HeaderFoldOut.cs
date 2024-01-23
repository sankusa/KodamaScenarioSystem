using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public static class HeaderFoldOut {
        public const int HeaderHeight = 20;
        public static bool Draw(Rect rect, string label, bool expand, Color color = default) {
            GUIStyle style = new GUIStyle("HeaderButton") {
                fixedHeight = HeaderHeight,
            };

            GUI.Box(rect, "", style);
            EditorGUI.DrawRect(new RectOffset(0, 0, 0, 1).Remove(rect), color);

            Rect labelRect = new Rect(rect) {xMin = rect.xMin + 20};
            EditorGUI.LabelField(labelRect, label);

            Event e = Event.current;

            if(e.type == EventType.Repaint) {
                Rect toggleRect = new Rect(rect.x + 4, rect.y + 2, 13, 13);
                EditorStyles.foldout.Draw(toggleRect, false, false, expand, false);
            }
            else if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                expand = !expand;
                e.Use();
            }

            return expand;
        }

        public static bool DrawLayout(string label, bool expand, Color color = default) {
            Rect rect = GUILayoutUtility.GetRect(0, HeaderHeight);
            return Draw(rect, label, expand, color);
        }

        public static bool DrawFoldoutGroupFrame(Rect rect, string label, bool expand) {
            GUIStyle frameStyle = new GUIStyle("GroupBox") {
                margin = new RectOffset(),
                padding = new RectOffset(),
            };
            GUI.Box(rect, "", frameStyle);

            Rect headerRect = new Rect(rect.x + 1, rect.y + 1, rect.width - 2, HeaderHeight);
            expand = Draw(headerRect, label, expand);

            return expand;
        }

        public static bool BeginLayoutFoldoutGroup(string label, bool expand, Color color = default) {
            GUIStyle frameStyle = new GUIStyle("GroupBox") {
                margin = new RectOffset(),
                padding = new RectOffset(1, 1, 1, 0),
            };
            EditorGUILayout.BeginVertical(frameStyle);
            expand = DrawLayout(label, expand, color);
            EditorGUI.indentLevel++;
            return expand;
        }

        public static void EndLayoutFoldoutGroup() {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}