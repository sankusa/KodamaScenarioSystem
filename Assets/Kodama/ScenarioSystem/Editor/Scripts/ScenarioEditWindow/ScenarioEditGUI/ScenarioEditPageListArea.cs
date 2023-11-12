using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditPageListArea {
        private ReorderableList _pageList;

        internal void DrawLayout(ScenarioEditGUIStatus status, Scenario scenario, SerializedObject serializedObject) {
            if(_pageList == null) {
                _pageList = new ReorderableList(serializedObject, serializedObject.FindProperty("_pages"));

                _pageList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, $"{scenario.Pages.Count} Pages");

                _pageList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    EditorGUI.LabelField(rect, scenario.Pages[index].Name);
                };

                _pageList.onSelectCallback = list => {
                    status.CurrentPageIndex = list.index;
                };

                _pageList.onAddCallback = list => {
                    SerializedProperty pagesProp = serializedObject.FindProperty("_pages");
                    pagesProp.InsertArrayElementAtIndex(pagesProp.arraySize);
                    SerializedProperty newPageProp = pagesProp.GetArrayElementAtIndex(pagesProp.arraySize - 1);
                    SerializedProperty commandsProp = newPageProp.FindPropertyRelative("_commands");
                    commandsProp.arraySize = 0;
                };

                _pageList.drawHeaderCallback = rect => {
                    Rect headerRect = new Rect(rect.x - 4, rect.y, rect.width + 9, rect.height);
                    GUI.Box(headerRect, "", GUIStyles.TitleBar);
                    EditorGUI.LabelField(headerRect, "ページリスト");
                };
            }

            _pageList.DoLayoutList();
        }
    }
}