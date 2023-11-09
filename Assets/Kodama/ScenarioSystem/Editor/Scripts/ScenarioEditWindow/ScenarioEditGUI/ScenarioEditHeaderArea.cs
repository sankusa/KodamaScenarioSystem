using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditHeaderArea {

        internal void DrawLayout(Scenario scenario) {
            var buttonStyle = new GUIStyle("AppToolbarButtonLeft");
            var richTextLabelStyle = new GUIStyle(EditorStyles.label);
            richTextLabelStyle.richText = true;

            using(new EditorGUILayout.HorizontalScope()) {
                if(GUILayout.Button("< Back", buttonStyle, GUILayout.Width(80), GUILayout.Height(24))) {
                    ScenarioEditWindow.Open();
                }
                //EditorGUILayout.LabelField($"<size=18>{scenario.name}</size>", richTextLabelStyle, GUILayout.Height(24));
                using(new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.ObjectField(scenario, typeof(Scenario), allowSceneObjects: false, GUILayout.Height(24));
                }
            }
            //GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
        } 
    }
}