using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class PageListArea {
        private ReorderableList _pageList;

        public void DrawLayout(ScenarioEditGUIStatus status, Scenario scenario, SerializedObject serializedObject) {
            if(_pageList == null) {
                _pageList = new ReorderableList(serializedObject, serializedObject.FindProperty("_pages")) {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, $"{scenario.Pages.Count} Pages"),

                    drawElementCallback = (rect, index, isActive, isFocused) => {
                        if(scenario.Pages[index] == scenario.DefaultPage) {
                            EditorGUI.LabelField(rect, scenario.Pages[index].name + " ---- <Default>");
                        }
                        else {
                            EditorGUI.LabelField(rect, scenario.Pages[index].name);
                        }
                    },

                    onSelectCallback = list => {
                        status.CurrentPageIndex = list.index;
                    },

                    onAddCallback = list => {
                        // ScenarioPage newPage = ScriptableObject.CreateInstance<ScenarioPage>();
                        // newPage.name = "New Page";
                        // newPage.Scenario = scenario;
                        // Undo.RegisterCreatedObjectUndo(newPage, "Add new page");
                        // AssetDatabase.AddObjectToAsset(newPage, scenario);

                        // SerializedProperty pagesProp = serializedObject.FindProperty("_pages");
                        // pagesProp.InsertArrayElementAtIndex(pagesProp.arraySize);
                        // SerializedProperty newPageProp = pagesProp.GetArrayElementAtIndex(pagesProp.arraySize - 1);
                        // newPageProp.objectReferenceValue = newPage;
                        scenario.CreatePage();
                    },

                    onRemoveCallback = list => {
                        SerializedProperty pagesProp = serializedObject.FindProperty("_pages");
                        if (list.index >= pagesProp.arraySize) return;
                        // SerializedProperty pageProp = pagesProp.GetArrayElementAtIndex(list.index);
                        // ScenarioPage page = pageProp.objectReferenceValue as ScenarioPage;
                        
                        // Undo.RecordObject(scenario, "Remove ScenarioPage");
                        // scenario.Pages.RemoveAt(list.index);
                        // Undo.DestroyObjectImmediate(page);
                        scenario.DestroyPageAt(list.index);
                    },
                };

                _pageList.drawHeaderCallback = rect => {
                    Rect headerRect = new Rect(rect.x - 4, rect.y, rect.width + 9, rect.height);
                    GUI.Box(headerRect, "", GUIStyles.TitleBar);
                    EditorGUI.LabelField(headerRect, "Page List");
                };
            }

            _pageList.DoLayoutList();
        }
    }
}