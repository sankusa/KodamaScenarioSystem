using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(CallScenarioCommand), true)]
    public class CallScenarioCommandInspector : AsyncCommandBaseInspector {
        private ReorderableList _scenarioArgList;

        private Dictionary<Type, VariableValueFieldBase> _customValueDrawerDic = new Dictionary<Type, VariableValueFieldBase>();

        public override void OnInspectorGUIMainForAsyncCommand() {
            CallScenarioCommand command = target as CallScenarioCommand;
            Scenario scenario = command.ParentPage.ParentScenario;
            SerializedProperty targetProp = serializedObject.FindProperty("_target");
            SerializedProperty callTypeProp = serializedObject.FindProperty("_callType");
            List<CallArg> scenarioArgs = command.ScenarioArgs;
            SerializedProperty scenarioArgsProp = serializedObject.FindProperty("_scenarioArgs");

            if(_customValueDrawerDic.Count == 0) {
                var drawerTypes = TypeCache.GetTypesWithAttribute(typeof(CustomVariableValueFieldAttribute));
                foreach(Type drawerType in drawerTypes) {
                    var attribute = drawerType.GetCustomAttribute<CustomVariableValueFieldAttribute>();
                    _customValueDrawerDic[attribute.Type] = (VariableValueFieldBase)Activator.CreateInstance(drawerType);
                }
            }

            if(_scenarioArgList == null) {
                _scenarioArgList = new ReorderableList(serializedObject, scenarioArgsProp);
                _scenarioArgList.onAddDropdownCallback = (buttonRect, list) =>  {
                    Scenario targetScenario = command.Target.Scenario;
                    if(targetScenario == null) return;

                    GenericMenu addCallArgMenu = new GenericMenu();
                    foreach(VariableBase variable in targetScenario.Variables) {
                        Type targetType = variable.GetType().BaseType.GenericTypeArguments[0];
                        string variableId = variable.Id;
                        // 追加済みの引数は飛ばす
                        if(scenarioArgs.Find(x => x.TargetType == variable.TargetType && x.VariableId == variable.Id) != null) continue;

                        addCallArgMenu.AddItem(
                            new GUIContent(variable.Name),
                            false,
                            () => {
                                Undo.RecordObject(command, "Add CallArg");
                                CallArg scenarioArg = Activator.CreateInstance(
                                    TypeCache.GetTypesDerivedFrom<CallArg>()
                                        .First(x => x.IsAbstract == false && x.IsGenericType == false && x.BaseType.GenericTypeArguments[0] == targetType)
                                ) as CallArg;
                                scenarioArg.VariableId = variableId;
                                scenarioArgs.Add(scenarioArg);
                            }
                        );
                    }
                    addCallArgMenu.DropDown(buttonRect);
                };

                _scenarioArgList.drawHeaderCallback = rect => {
                    EditorGUI.LabelField(rect, "Scenario Args");
                };

                _scenarioArgList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    Rect[] rects = RectUtil.DivideRectHorizontal(rect, new RectUtil.LayoutLength[]{new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(2)});
                    CallArg callArg = scenarioArgs[index];
                    Scenario targetScenario = command.Target.Scenario;
                    if(targetScenario != null) {
                        VariableBase targetVariable = targetScenario.Variables.FirstOrDefault(x => x.TargetType == callArg.TargetType && x.Id == callArg.VariableId);
                        if(targetVariable != null) {
                            EditorGUI.LabelField(rects[0],  TypeNameUtil.ConvertToPrimitiveTypeName(targetVariable.TargetType.Name));
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUI.TextField(rects[1],  targetVariable.Name);
                            EditorGUI.EndDisabledGroup();
                            if(_customValueDrawerDic.ContainsKey(targetVariable.TargetType)) {
                                _customValueDrawerDic[targetVariable.TargetType].Draw(rects[2], command, callArg);
                            } 
                            else {
                                SerializedProperty argValueProp = scenarioArgsProp.GetArrayElementAtIndex(index).FindPropertyRelative("_value");
                                EditorGUI.PropertyField(rects[2], argValueProp, GUIContent.none);
                            }
                           
                        }
                    }
                };
            }
            EditorGUILayout.PropertyField(targetProp);
            EditorGUILayout.PropertyField(callTypeProp);

            _scenarioArgList.DoLayoutList();
        }
    }
}