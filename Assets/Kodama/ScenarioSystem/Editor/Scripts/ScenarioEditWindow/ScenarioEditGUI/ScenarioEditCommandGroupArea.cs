using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditCommandGroupArea {
        private Vector2 _scrollPos;
        public void DrawLayout(ScenarioEditGUIStatus guiStatus) {
            var buttonStyle = new GUIStyle(GUI.skin.button);
            //buttonStyle.stretchWidth = false;
            //buttonStyle.width;
            using var vs = new EditorGUILayout.VerticalScope();
            
            //EditorGUILayout.LabelField("コマンド分類");
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach(CommandGroupSetting groupSetting in CommandGroupSettingTable.AllSettings) {
                using var bgs = new BackgroundColorScope(groupSetting.GroupColor);
                if(GUILayout.Button(groupSetting.DisplayName, buttonStyle)) {
                    guiStatus.CurrentCommandGroupId = groupSetting.GroupId;
                }
            }

            EditorGUILayout.EndScrollView();
        } 
    }
}