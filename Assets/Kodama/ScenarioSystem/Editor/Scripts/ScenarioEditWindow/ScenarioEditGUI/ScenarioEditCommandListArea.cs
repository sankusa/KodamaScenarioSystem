using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using Codice.CM.Common.Tree;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditCommandListArea {
        private Vector2 _scrollPos;

        public void DrawLayout(ScenarioEditGUIStatus guiStatus, SerializedObject serializedPage) {
            ScenarioPage page = serializedPage.targetObject as ScenarioPage;
            CommandGroupSetting groupSetting = CommandGroupSetting.All.FirstOrDefault(x => x.name == guiStatus.CurrentCommandGroupSettingName);

            if(groupSetting == null) return;

            using var _ =  new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true));

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach(CommandSetting setting in groupSetting.CommandSettings) {
                Rect buttonRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
                if(GUI.Button(buttonRect, "", GUIStyles.BorderedButton)) {
                    int insertIndex = guiStatus.CurrentCommandIndex + 1;
                    page.InsertCommand(insertIndex, CommandBase.CreateInstance(setting.CommandScript.GetClass(), page));
                    guiStatus.CurrentCommandIndex++;
                }

                Rect buttonInnerRect = RectUtil.Margin(buttonRect, 1, 1, 1, 1);

                Color buttonColor = groupSetting.Color;
                buttonColor.a = 0.7f;
                EditorGUI.DrawRect(buttonInnerRect, buttonColor);

                Rect rect = GUILayoutUtility.GetLastRect();
                Rect iconRect = new Rect(rect.xMin, rect.yMin, 20, 20);
                Rect nameRect = new Rect(rect.xMin + 20, rect.yMin, rect.width - 20, 20);
                using (new ContentColorScope(setting.IconColor)) {
                    EditorGUI.LabelField(iconRect, new GUIContent(setting.Icon, null));
                }
                EditorGUI.LabelField(nameRect, setting.DisplayName);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}