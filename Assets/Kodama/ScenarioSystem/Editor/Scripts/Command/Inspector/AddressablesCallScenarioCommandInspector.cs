using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif

namespace Kodama.ScenarioSystem.Editor {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
    [CustomEditor(typeof(AddressablesCallScenarioCommand), true)]
    public class AddressablesCallScenarioCommandInspector : AsyncCommandBaseInspector {
        private ReorderableList _scenarioArgList;

        private Dictionary<Type, VariableValueFieldBase> _customValueDrawerDic = new Dictionary<Type, VariableValueFieldBase>();

        public override void OnInspectorGUIMainForAsyncCommand() {
            AddressablesCallScenarioCommand command = target as AddressablesCallScenarioCommand;
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
                    Scenario targetScenario = command.Target.editorAsset;
                    if(targetScenario == null) return;

                    GenericMenu addCallArgMenu = new GenericMenu();
                    foreach(VariableBase variable in targetScenario.Variables) {
                        // 追加済みの引数は飛ばす
                        if(scenarioArgs.Find(x => variable.IsMatch(x.TargetType, x.VariableId)) != null) continue;

                        addCallArgMenu.AddItem(
                            new GUIContent(variable.Name),
                            false,
                            () => {
                                Undo.RecordObject(command, "Add CallArg");
                                CallArg scenarioArg = Activator.CreateInstance(
                                    TypeCache.GetTypesDerivedFrom<CallArg>()
                                        .First(x => x.IsAbstract == false && x.IsGenericType == false && x.BaseType.GenericTypeArguments[0] == variable.TargetType)
                                ) as CallArg;
                                scenarioArg.VariableId = variable.Id;
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
                    Scenario targetScenario = command.Target.editorAsset;
                    if(targetScenario != null) {
                        VariableBase targetVariable = targetScenario.Variables.FirstOrDefault(x => x.IsMatch(callArg.TargetType, callArg.VariableId));
                        if(targetVariable != null) {
                            EditorGUI.LabelField(new Rect(rects[0]) {height = EditorGUIUtility.singleLineHeight},  TypeNameUtil.ConvertToPrimitiveTypeName(targetVariable.TargetType.Name));
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUI.TextField(new Rect(rects[1]) {height = EditorGUIUtility.singleLineHeight, width = rects[1].width - EditorGUIUtility.standardVerticalSpacing},  targetVariable.Name);
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

                _scenarioArgList.elementHeightCallback = index => {
                    CallArg callArg = scenarioArgs[index];
                    Scenario targetScenario = command.Target.editorAsset;
                    if(targetScenario != null) {
                        VariableBase targetVariable = targetScenario.Variables.FirstOrDefault(x => x.IsMatch(callArg.TargetType, callArg.VariableId));
                        if(targetVariable != null) {
                            if(_customValueDrawerDic.ContainsKey(targetVariable.TargetType)) {
                                return _customValueDrawerDic[targetVariable.TargetType].GetHeight();
                            }
                        }
                    }
                    return EditorGUIUtility.singleLineHeight;
                };
            }
            EditorGUILayout.PropertyField(targetProp);
            EditorGUILayout.PropertyField(callTypeProp);

            _scenarioArgList.DoLayoutList();
        }
    }
#endif
}