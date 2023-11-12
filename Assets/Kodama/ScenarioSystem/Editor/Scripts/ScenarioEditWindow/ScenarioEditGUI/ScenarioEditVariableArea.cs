using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace Kodama.ScenarioSystem.Editor {
    
    
    public class ScenarioEditVariableArea {
        private ReorderableList _variableList;
        private Vector2 _scrollPos;

        internal void DrawLayout(ScenarioEditGUIStatus status, Scenario scenario, SerializedObject serializedObject) {
            if(_variableList == null) {
                SerializedProperty variablesProp = serializedObject.FindProperty("_variables");
                _variableList = new ReorderableList(serializedObject, variablesProp);

                _variableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, $"{scenario.Variables.Count} Variables");

                _variableList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    EditorGUI.PropertyField(rect, variablesProp.GetArrayElementAtIndex(index));
                };

                _variableList.elementHeightCallback = index => {
                    return EditorGUI.GetPropertyHeight(variablesProp.GetArrayElementAtIndex(index));
                };

                _variableList.onSelectCallback = list => {
                    //status.CurrentPageIndex = list.index;
                };

                _variableList.onAddDropdownCallback = (buttonRect, list) => {
                    GenericMenu menu = new GenericMenu();
                    foreach(var t in TypeCache.GetTypesDerivedFrom<VariableBase>().Where(t => !t.IsGenericType)) {
                        string typeName = t.BaseType
                            .GetField("_value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .FieldType
                            .Name;
                        typeName = TypeNameUtil.ConvertToPrimitiveTypeName(typeName);
                        menu.AddItem(new GUIContent(typeName), false, () => scenario.Variables.Add((VariableBase)Activator.CreateInstance(t)));
                    }
                    // foreach(VariableSetting setting in VariableSettingTable.AllSettings) {
                    //     menu.AddItem(new GUIContent(setting.DisplayName), false, () => scenario.Variables.Add((VariableBase)Activator.CreateInstance(setting.NonGenericVariableScript.GetClass())));
                    // }
                    menu.DropDown(buttonRect);
                };

                _variableList.drawHeaderCallback = rect => {
                    Rect headerRect = new Rect(rect.x - 4, rect.y, rect.width + 9, rect.height);
                    GUI.Box(headerRect, "", GUIStyles.TitleBar);
                    EditorGUI.LabelField(headerRect, "変数");
                };

            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                _variableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
    }
}