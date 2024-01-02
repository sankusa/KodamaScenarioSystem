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
            IEnumerable<CommandSetting> settings = CommandSettingTable.AllSettings.Where(x => x.GroupId == guiStatus.CurrentCommandGroupId);
            CommandGroupSetting groupSetting = CommandGroupSettingTable.AllSettings.FirstOrDefault(x => x.GroupId == guiStatus.CurrentCommandGroupId);

            if(groupSetting == null) return;

            using var _ =  new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true));

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach(CommandSetting setting in settings) {
                GUILayout.Box("", GUIStyles.CommandListElementStyle, GUILayout.Height(20));

                Rect rect = GUILayoutUtility.GetLastRect();
                Rect iconRect = new Rect(rect.xMin, rect.yMin, 20, 20);
                Rect nameRect = new Rect(rect.xMin + 20, rect.yMin, rect.width - 40, 20);
                Rect addButtonRect = new Rect(rect.xMax - 20, rect.yMin, 20, 20);
                // Rect insertButtonRect = new Rect(rect.xMax - 20, rect.yMin, 20, 20);
                using (new ContentColorScope(groupSetting.GroupColor)) {
                    EditorGUI.LabelField(iconRect, new GUIContent(setting.Icon, null));
                }
                EditorGUI.LabelField(nameRect, setting.DisplayName);
                if(GUI.Button(addButtonRect, CommonEditorResources.Instance.CommandAddIcon)) {
                    Undo.RecordObject(page, "Add Command");
                    int insertIndex = guiStatus.CurrentCommandIndex + 1;
                    insertIndex = Mathf.Clamp(insertIndex, 0, page.Commands.Count);
                    page.InsertCommand(insertIndex, CommandBase.CreateInstance(setting.CommandScript.GetClass(), page));
                    guiStatus.CurrentCommandIndex++;
                }
                // if(GUI.Button(insertButtonRect, CommonEditorResources.Instance.CommandInsertIcon)) {
                //     Undo.RecordObject(page, "Insert Command");
                //     page.InsertCommand(guiStatus.CurrentCommandIndex, CommandBase.CreateInstance(setting.CommandScript.GetClass(), page));
                // }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}