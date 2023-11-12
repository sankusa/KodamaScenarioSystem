using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditHeaderArea {

        internal void DrawLayout(Scenario scenario, SerializedObject serializedObject) {
            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("< Back", GUIStyles.BorderedButton, GUILayout.Width(80), GUILayout.Height(24))) {
                    ScenarioEditWindow.Open();
                }
                using(new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.ObjectField(scenario, typeof(Scenario), allowSceneObjects: false, GUILayout.Height(24));
                }
            }
        } 
    }
}