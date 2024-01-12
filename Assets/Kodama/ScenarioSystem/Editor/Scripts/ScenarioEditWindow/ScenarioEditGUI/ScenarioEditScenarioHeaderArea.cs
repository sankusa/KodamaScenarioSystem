using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditScenarioHeaderArea {
        public void DrawLayout(Scenario scenario, SerializedObject serializedObject) {
            using(new EditorGUILayout.HorizontalScope()) {
                // シナリオ名
                using (new ContentColorScope(new Color(1, 1, 1, 0.8f))) {
                    EditorGUI.BeginChangeCheck();
                    string newName = EditorGUILayout.DelayedTextField(GUIContent.none, scenario.name, GUIStyles.ScenarioNameTextField);
                    if(EditorGUI.EndChangeCheck()) {
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(scenario), newName);
                    }
                }
                // ObjectField
                using(new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.ObjectField(scenario, typeof(Scenario), allowSceneObjects: false, GUILayout.Height(24), GUILayout.Width(35));
                }
                // 戻る
                if(GUILayout.Button("< Back", GUIStyles.BorderedButton, GUILayout.Width(80), GUILayout.Height(23))) {
                    ScenarioEditWindow.Open();
                }
                GUILayout.Space(3);
            }
        } 
    }
}