using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    
    
    public class VariableArea {
        private ReorderableList _variableList;
        private Vector2 _scrollPos;
        private VariableDrawer _variableDrawer = new VariableDrawer();

        internal void DrawLayout(ScenarioEditGUIStatus status, Scenario scenario, SerializedObject serializedObject) {
            if(_variableList == null) {
                SerializedProperty variablesProp = serializedObject.FindProperty("_variables");
                _variableList = new ReorderableList(serializedObject, variablesProp);

                _variableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, $"{scenario.Variables.Count} Variables");

                _variableList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    _variableDrawer.OnGUI(rect, variablesProp.GetArrayElementAtIndex(index), scenario.Variables[index]);
                };

                _variableList.elementHeightCallback = index => {
                    return _variableDrawer.GetPropertyHeight(variablesProp.GetArrayElementAtIndex(index), scenario.Variables[index]);
                };

                _variableList.onSelectCallback = list => {
                    //status.CurrentPageIndex = list.index;
                };

                _variableList.onAddDropdownCallback = (buttonRect, list) => {
                    GenericMenu menu = new GenericMenu();
                    foreach(var t in TypeCache.GetTypesDerivedFrom<VariableBase>().Where(t => !t.IsGenericType)) {
                        string typeName = t.BaseType
                            .GetField(Variable<object>.VariableName_Value, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .FieldType
                            .Name;
                        typeName = TypeNameUtil.ConvertToPrimitiveTypeName(typeName);
                        menu.AddItem(new GUIContent(typeName), false, () => scenario.Variables.Add((VariableBase)Activator.CreateInstance(t)));
                    }
                    menu.DropDown(buttonRect);
                };

                _variableList.drawHeaderCallback = rect => {
                    Rect headerRect = new Rect(rect.x - 4, rect.y, rect.width + 9, rect.height);
                    GUI.Box(headerRect, "", GUIStyles.TitleBar);
                    EditorGUI.LabelField(headerRect, "Variable Define");
                };

            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                _variableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
    }
}