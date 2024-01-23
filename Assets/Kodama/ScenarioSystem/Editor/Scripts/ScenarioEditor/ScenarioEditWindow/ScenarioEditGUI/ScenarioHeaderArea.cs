using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class ScenarioHeaderArea {
        public void DrawLayout(Scenario scenario, SerializedObject serializedObject) {
            using(new EditorGUILayout.HorizontalScope()) {
                // // ObjectField
                // using(new EditorGUI.DisabledGroupScope(true)) {
                //     EditorGUILayout.ObjectField(scenario, typeof(Scenario), allowSceneObjects: false, GUILayout.Height(24), GUILayout.Width(35));
                // }

                // シナリオ名
                using (new ContentColorScope(new Color(1, 1, 1, 0.8f))) {
                    EditorGUI.BeginChangeCheck();
                    string newName = EditorGUILayout.DelayedTextField(GUIContent.none, scenario.name, GUIStyles.ScenarioNameTextField);
                    if(EditorGUI.EndChangeCheck()) {
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(scenario), newName);
                    }
                }

                if(GUILayout.Button("Scenarios", GUIStyles.BorderedButton, GUILayout.Width(64), GUILayout.Height(23))) {
                    ScenarioListWindow.Open();
                }
                if(GUILayout.Button("Select", GUIStyles.BorderedButton, GUILayout.Width(64), GUILayout.Height(23))) {
                    Selection.activeObject = scenario;
                }
                
                GUILayout.Space(3);
            }


        } 
    }
}