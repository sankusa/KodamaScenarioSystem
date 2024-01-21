using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class CommandGroupArea {
        private Vector2 _scrollPos;
        public void DrawLayout(ScenarioEditGUIStatus guiStatus) {
            using var vs = new EditorGUILayout.VerticalScope();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach(CommandGroupSetting groupSetting in CommandGroupSetting.All) {
                Rect buttonRect = GUILayoutUtility.GetRect(0, 22, GUILayout.ExpandWidth(true));
                Rect buttonInnerRect = RectUtil.Margin(buttonRect, 1, 1, 1, 1);

                Color buttonColor = groupSetting.Color;
                buttonColor.a = 0.7f;
                
                if(GUI.Button(buttonRect, "", GUIStyles.BorderedButton)) {
                    guiStatus.CurrentCommandGroupSettingName = groupSetting.name;
                }
                EditorGUI.DrawRect(buttonInnerRect, buttonColor);
                EditorGUI.LabelField(buttonInnerRect, groupSetting.DisplayName, GUIStyles.CenteredLabel);
            }

            EditorGUILayout.EndScrollView();
        } 
    }
}