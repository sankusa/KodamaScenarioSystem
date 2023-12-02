using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditPageHeaderArea {
        public void DrawLayout(SerializedProperty pageProp, int index, int pageCount) {
            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField($"ページ {index + 1}/{pageCount}", GUILayout.Width(80));
                EditorGUILayout.LabelField($"ページ名 : ", GUILayout.Width(54));
                EditorGUILayout.PropertyField(pageProp.FindPropertyRelative("_name"), GUIContent.none, GUILayout.ExpandWidth(true));
            }
        }
    }
}