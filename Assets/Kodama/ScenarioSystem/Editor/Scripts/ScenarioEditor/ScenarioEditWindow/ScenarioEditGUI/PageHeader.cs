using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class PageHeaderArea {
        public void DrawLayout(SerializedObject serializedPage) {
            EditorGUI.BeginChangeCheck();
            SerializedProperty nameProp = serializedPage.FindProperty("m_Name");
            using (new ContentColorScope(new Color(1, 1, 1, 0.8f))) {
                nameProp.stringValue = EditorGUILayout.DelayedTextField(GUIContent.none, nameProp.stringValue, GUIStyles.BoldTextField, GUILayout.Height(20));
            }
            if(EditorGUI.EndChangeCheck()) {
                serializedPage.ApplyModifiedProperties();
            }
        } 
    }
}