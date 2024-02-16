using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(ReflectionMethodInvokeData))]
    public class ReflectionCommandParamtersDrawer : PropertyDrawer {
        private Assembly[] _assemblies;
        private string[] _assemblyPopupOptions;

        private Type[] _types;
        private string[] _typePopupOptions;

        private MethodInfo[] _methodInfos;
        private string[] _methodMenuLabels;

        private ValueStringConverterBundle _converters = new ValueStringConverterBundle();
        private Type[] _allowedVariableTypes;
        private StringBuilder _sb = new StringBuilder();

        private const string _undoRedoNameOnChange = "ReflectionMethodInvokeData changed";

        private static readonly RectUtil.LayoutLength[] _argRowWidths = new RectUtil.LayoutLength[] {
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(0.5f),
            new RectUtil.LayoutLength(1.5f)
        };

        public ReflectionCommandParamtersDrawer() : base() {
            UpdateAssemblies();
            _allowedVariableTypes = TypeCache.GetTypesDerivedFrom<VariableBase>().Where(t => !t.IsGenericType).Select(x => x.BaseType.GenericTypeArguments[0]).ToArray();
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            OnGUIMain(ref position, property, label);
            OnGUISummary(position, property);
        }
        private void OnGUIMain(ref Rect position, SerializedProperty property, GUIContent label) {
            CommandBase command = property.serializedObject.targetObject as CommandBase;
            ReflectionMethodInvokeData invokeData = property.GetObject() as ReflectionMethodInvokeData;

            // アセンブリ
            Rect rect1 = CutSingleLineRect(ref position);
            int assemblyIndex = Array.FindIndex(_assemblies, x => x.GetName().Name == invokeData.TypeId.AssemblyName);
            assemblyIndex = EditorGUI.Popup(rect1, "Assembly Name", assemblyIndex, _assemblyPopupOptions);
            string newAssemblyName = "";
            if(assemblyIndex > -1) {
                newAssemblyName = _assemblies[assemblyIndex].GetName().Name;
            }
            if(newAssemblyName != invokeData.TypeId.AssemblyName) {
                // 変更があれば、以降の情報をクリア
                Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                invokeData.TypeId.AssemblyName = newAssemblyName;
                invokeData.TypeId.TypeFullName = "";
                invokeData.MethodType = MethodType.Instance;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();

                // エディタの更新
                UpdateTypes(assemblyIndex);
            }

            if(string.IsNullOrEmpty(invokeData.TypeId.AssemblyName)) return;

            // 型名
            Rect rect2 = CutSingleLineRect(ref position);
            if(_types == null) {
                UpdateTypes(assemblyIndex);
            }
            
            int typeIndex = Array.FindIndex(_types, x => x.FullName == invokeData.TypeId.TypeFullName);
            typeIndex = EditorGUI.Popup(rect2, "Type Name", typeIndex, _typePopupOptions);
            string newTypeName = "";
            if(typeIndex > -1) {
                newTypeName = _types[typeIndex].FullName;
            }
            if(newTypeName != invokeData.TypeId.TypeFullName) {
                Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                invokeData.TypeId.TypeFullName = newTypeName;
                invokeData.MethodType = MethodType.Instance;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();

                UpdateMethods(_types[typeIndex], MethodType.Instance);
            }

            if(string.IsNullOrEmpty(invokeData.TypeId.TypeFullName)) return;

            // Instance Or Static
            Rect rect3 = CutSingleLineRect(ref position);
            MethodType newMethodType = (MethodType) EditorGUI.EnumPopup(rect3, "Method Type",  invokeData.MethodType);
            if(newMethodType != invokeData.MethodType) {
                Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                invokeData.MethodType = newMethodType;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();

                UpdateMethods(_types[typeIndex], invokeData.MethodType);
            }

            // インスタンスの参照取得方法
            if(invokeData.MethodType == MethodType.Instance) {
                Rect rect4 = CutSingleLineRect(ref position);
                TargetInstanceResolveWay newInstanceResolveWay = (TargetInstanceResolveWay) EditorGUI.EnumPopup(rect4, "Target Instance",  invokeData.InstanceResolveWay);
                if(newInstanceResolveWay != invokeData.InstanceResolveWay) {
                    Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                    invokeData.InstanceResolveWay = newInstanceResolveWay;
                }
            }

            // メソッド
            Rect rect5 = CutSingleLineRect(ref position);
            string methodSelectButtonLabel = "Select Method";
            if(string.IsNullOrEmpty(invokeData.MethodData.MethodName) == false) {
                MethodInfo methodInfo = invokeData.TypeId.ResolveType()
                    .GetMethod(
                        invokeData.MethodData.MethodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                        null,
                        invokeData.MethodData.ArgData.Select(x => x.TypeId.ResolveType()).ToArray(),
                        null);
                methodSelectButtonLabel = CreateMethodLabel(methodInfo);
            }
            if(GUI.Button(rect5, methodSelectButtonLabel)) {
                GenericMenu methodMenu = new GenericMenu();

                if(_methodInfos == null) {
                    UpdateMethods(_types[typeIndex], invokeData.MethodType);
                }

                for(int i = 0; i < _methodInfos.Length; i++) {
                    int index = i;
                    methodMenu.AddItem(
                        new GUIContent(_methodMenuLabels[i]),
                        false,
                        () => {
                            Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                            invokeData.MethodData = new MethodData(_methodInfos[index], _converters);
                        }
                    );
                }

                methodMenu.ShowAsContext();
            }

            // メソッド内容
            if(string.IsNullOrEmpty(invokeData.MethodData.MethodName) == false) {
                MethodInfo methodInfo = invokeData.TypeId.ResolveType()
                    .GetMethod(
                        invokeData.MethodData.MethodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                        null,
                        invokeData.MethodData.ArgData.Select(x => x.TypeId.ResolveType()).ToArray(),
                        null);

                // 引数
                int parameterInfoIndex = 0;
                foreach(MethodArgData data in invokeData.MethodData.ArgData) {
                    Rect dataRect = CutSingleLineRect(ref position);
                    Rect[] dataRects = RectUtil.DivideRectHorizontal(dataRect, _argRowWidths, 0, 0);

                    Type argType = data.TypeId.ResolveType();
                    ValueStringConverterBase converter = _converters.FindConverter(argType);

                    ParameterInfo parameterInfo = methodInfo.GetParameters()[parameterInfoIndex++];

                    EditorGUI.LabelField(dataRects[0], TypeNameUtil.ConvertToPrimitiveTypeName(data.TypeId.TypeName));
                    EditorGUI.LabelField(dataRects[1], data.ArgName);

                    MethodArgResolveWay newResolveWay =
                        (MethodArgResolveWay) EditorGUI.EnumPopup(
                            dataRects[2],
                            GUIContent.none,
                            data.ResolveWay,
                            x => {
                                if((MethodArgResolveWay)x == MethodArgResolveWay.Value && converter == null) return false;
                                else if((MethodArgResolveWay)x == MethodArgResolveWay.Variable && _allowedVariableTypes.Contains(argType) == false) return false;
                                else if((MethodArgResolveWay)x == MethodArgResolveWay.DefaultValue && parameterInfo.HasDefaultValue == false) return false;
                                return true;
                            }
                        );
                    if(newResolveWay != data.ResolveWay) {
                        Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                        data.ResolveWay = newResolveWay;
                    }

                    if(data.ResolveWay == MethodArgResolveWay.Value) {
                        if(converter == null) {
                            EditorGUI.LabelField(dataRects[3], "----");
                        }
                        else {
                            string newValueString = converter.DrawField(dataRects[3], "", data.ValueString);
                            if(newValueString != data.ValueString) {
                                Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                                data.ValueString = newValueString;
                            }
                        }
                    }
                    else if(data.ResolveWay == MethodArgResolveWay.Variable) {
                        string newVariableId = VariableSelector(dataRects[3], command, parameterInfo.ParameterType, data.VaribaleId);
                        if(newVariableId != data.VaribaleId) {
                            Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                            data.VaribaleId = newVariableId;
                        }
                    }
                }

                // 戻り値のハンドリング
                Rect returnValueLabelRect = CutSingleLineRect(ref position);
                EditorGUI.LabelField(returnValueLabelRect, "Return Value (" + TypeNameUtil.ConvertToPrimitiveTypeName(methodInfo.ReturnType.Name) + ")");

                Rect returnValueHandlingEditRect = CutSingleLineRect(ref position);
                Rect[] returnValueHandlingEditRects = RectUtil.DivideRectHorizontal(returnValueHandlingEditRect, new RectUtil.LayoutLength[] {new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1)});
                ReturnValueHandling newReturnValueHandling =
                    (ReturnValueHandling) EditorGUI.EnumPopup(
                        returnValueHandlingEditRects[0],
                        new GUIContent(),
                        invokeData.MethodData.ReturnValueHandling,
                        x => {
                            if((ReturnValueHandling)x == ReturnValueHandling.SetToVariable && _allowedVariableTypes.Contains(methodInfo.ReturnType) == false) return false;
                            else if((ReturnValueHandling)x == ReturnValueHandling.BindToServiceLocater && methodInfo.ReturnType == typeof(void)) return false;
                            return true;
                        }
                    );

                if(newReturnValueHandling != invokeData.MethodData.ReturnValueHandling) {
                    Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                    invokeData.MethodData.ReturnValueHandling = newReturnValueHandling;
                }

                if(invokeData.MethodData.ReturnValueHandling == ReturnValueHandling.SetToVariable) {
                    string newVariableId = VariableSelector(returnValueHandlingEditRects[1], command, methodInfo.ReturnType, invokeData.MethodData.ReturnValueReceiveVariableId);
                    if(newVariableId != invokeData.MethodData.ReturnValueReceiveVariableId) {
                        Undo.RecordObject(property.serializedObject.targetObject, _undoRedoNameOnChange);
                        invokeData.MethodData.ReturnValueReceiveVariableId = newVariableId;
                    }
                }
            }
        }

        private void OnGUISummary(Rect position, SerializedProperty property) {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_summary"));
        }

        private void UpdateAssemblies() {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(x => x.GetName().Name).ToArray();
            _assemblyPopupOptions = _assemblies.Select(x => x.GetName().Name.Replace(".", "/")).ToArray();
            _types = null;
            _typePopupOptions = null;
            _methodInfos = null;
            _methodMenuLabels = null;
        }

        private void UpdateTypes(int assemblyIndex) {
            // コンパイラが自動生成するクラス、ジェネリッククラスは弾く
            string removePattern = @".*[<`\+]+.*";
            _types = _assemblies[assemblyIndex].GetTypes().Where(x => Regex.IsMatch(x.FullName, removePattern) == false).OrderBy(x => x.FullName).ToArray();
            _typePopupOptions = _types.Select(x => x.FullName.Replace(".", "/")).ToArray();
            _methodInfos = null;
            _methodMenuLabels = null;
        }

        private void UpdateMethods(Type type, MethodType methodType) {
            if(methodType == MethodType.Instance) {
                _methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.IsGenericMethod == false && x.ContainsGenericParameters == false)
                    .ToArray();
            }
            else if(methodType == MethodType.Static) {
                _methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.IsGenericMethod == false && x.ContainsGenericParameters == false)
                    .ToArray();
            }
            _methodMenuLabels = _methodInfos.Select(CreateMethodLabel)
                .ToArray();
        }

        private string CreateMethodLabel(MethodInfo methodInfo) {
            _sb.Append(TypeNameUtil.ConvertToPrimitiveTypeName(methodInfo.ReturnType.Name));
            _sb.Append(" ");
            _sb.Append(methodInfo.Name);
            _sb.Append("(");
            ParameterInfo[] parameters = methodInfo.GetParameters();
            for(int i = 0; i < parameters.Length; i++) {
                if(i != 0) _sb.Append(", ");
                _sb.Append(TypeNameUtil.ConvertToPrimitiveTypeName(parameters[i].ParameterType.Name));
                _sb.Append(" ");
                _sb.Append(parameters[i].Name);
            }
            //if(parameters.Length == 0) _sb.Append("void");
            _sb.Append(")");
            string methodLabel = _sb.ToString();
            _sb.Clear();

            return methodLabel;
        }

        private string VariableSelector(Rect rect, CommandBase command, Type variableType, string variableId) {
            IEnumerable<VariableBase> variables = command.GetAvailableVariableDefines(variableType).ToArray();
            string[] variableNames = variables.Select(x => x.Name).ToArray();
            string[] variableIds = variables.Select(x => x.Id).ToArray();
            int variableIdIndex = Array.IndexOf(variableIds, variableId);
            variableIdIndex = EditorGUI.Popup(rect, variableIdIndex, variableNames);
            string newVariableId = "";
            if(variableIdIndex > -1) {
                newVariableId = variableIds[variableIdIndex];
            }
            return newVariableId;
        }

        public Rect CutSingleLineRect(ref Rect rect) {
            Rect ret = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return ret;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            ReflectionMethodInvokeData invokeData = property.GetObject() as ReflectionMethodInvokeData;
            float singleLineHeightWithSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float height = 0;
            height += singleLineHeightWithSpace;
            if(string.IsNullOrEmpty(invokeData.TypeId.AssemblyName) == false) {
                height += singleLineHeightWithSpace;
            }
            if(string.IsNullOrEmpty(invokeData.TypeId.TypeFullName) == false) {
                height += singleLineHeightWithSpace * 2;
                if(invokeData.MethodType == MethodType.Instance) height += singleLineHeightWithSpace;
                if(string.IsNullOrEmpty(invokeData.MethodData.MethodName) == false) {
                    height += singleLineHeightWithSpace * 2;
                    foreach(MethodArgData argData in invokeData.MethodData.ArgData) {
                        height += singleLineHeightWithSpace;
                    }
                }
            }

            SerializedProperty summaryProp = property.FindPropertyRelative("_summary");
            height += EditorGUI.GetPropertyHeight(summaryProp);
            return height;
        }
    }
}