using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(ReflectionMethodInvokeData))]
    public class ReflectionCommandParamtersDrawer : PropertyDrawer {
        private Assembly[] _assemblies;
        private string[] _assemblyOptions;

        private ValueStringConverterBundle _converters = new ValueStringConverterBundle();
        private Type[] _allowedVariableTypes;
        private StringBuilder _sb = new StringBuilder();

        private static readonly RectUtil.LayoutLength[] _argRowWidths = new RectUtil.LayoutLength[] {
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(0.5f),
            new RectUtil.LayoutLength(1.5f)
        };

        public ReflectionCommandParamtersDrawer() : base() {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(x => x.GetName().Name).ToArray();
            _assemblyOptions = _assemblies.Select(x => x.GetName().Name.Replace(".", "/")).ToArray();
            _allowedVariableTypes = TypeCache.GetTypesDerivedFrom<VariableBase>().Where(t => !t.IsGenericType).Select(x => x.BaseType.GenericTypeArguments[0]).ToArray();
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CommandBase command = property.serializedObject.targetObject as CommandBase;
            ReflectionMethodInvokeData invokeData = property.GetObject() as ReflectionMethodInvokeData;

            // アセンブリ
            Rect rect1 = CutSingleLineRect(ref position);
            int assemblyIndex = Array.FindIndex(_assemblies, x => x.GetName().Name == invokeData.TypeId.AssemblyName);
            assemblyIndex = EditorGUI.Popup(rect1, "Assembly Name", assemblyIndex, _assemblyOptions);
            string newAssemblyName = "";
            if(assemblyIndex > -1) {
                newAssemblyName = _assemblies[assemblyIndex].GetName().Name;
            }
            if(newAssemblyName != invokeData.TypeId.AssemblyName) {
                // 変更があれば、以降の情報をクリア
                Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                invokeData.TypeId.AssemblyName = newAssemblyName;
                invokeData.TypeId.TypeFullName = "";
                invokeData.MethodType = MethodType.Instance;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();
            }

            if(string.IsNullOrEmpty(invokeData.TypeId.AssemblyName)) return;

            // 型名
            Rect rect2 = CutSingleLineRect(ref position);
            Type[] types = _assemblies[assemblyIndex].GetTypes().OrderBy(x => x.FullName).ToArray();
            string[] typeOptions = types.Select(x => x.FullName.Replace(".", "/")).ToArray();
            int typeIndex = Array.FindIndex(types, x => x.FullName == invokeData.TypeId.TypeFullName);
            typeIndex = EditorGUI.Popup(rect2, "Type Name", typeIndex, typeOptions);
            string newTypeName = "";
            if(typeIndex > -1) {
                newTypeName = types[typeIndex].FullName;
            }
            if(newTypeName != invokeData.TypeId.TypeFullName) {
                Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                invokeData.TypeId.TypeFullName = newTypeName;
                invokeData.MethodType = MethodType.Instance;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();
            }

            if(string.IsNullOrEmpty(invokeData.TypeId.TypeFullName)) return;

            // Instance Or Static
            Rect rect3 = CutSingleLineRect(ref position);
            MethodType newMethodType = (MethodType) EditorGUI.EnumPopup(rect3, "Method Type",  invokeData.MethodType);
            if(newMethodType != invokeData.MethodType) {
                Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                invokeData.MethodType = newMethodType;
                invokeData.InstanceResolveWay = TargetInstanceResolveWay.FromServiceLocater;
                invokeData.MethodData.Clear();
            }

            // インスタンスの参照取得方法
            if(invokeData.MethodType == MethodType.Instance) {
                Rect rect4 = CutSingleLineRect(ref position);
                TargetInstanceResolveWay newInstanceResolveWay = (TargetInstanceResolveWay) EditorGUI.EnumPopup(rect4, "Target Instance",  invokeData.InstanceResolveWay);
                if(newInstanceResolveWay != invokeData.InstanceResolveWay) {
                    Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                    invokeData.InstanceResolveWay = newInstanceResolveWay;
                }
            }

            // メソッド
            Rect rect5 = CutSingleLineRect(ref position);
            if(GUI.Button(rect5, "Select Method")) {
                GenericMenu methodMenu = new GenericMenu();

                MethodInfo[] methodInfos = null;
                if(invokeData.MethodType == MethodType.Instance) {
                    methodInfos = types[typeIndex].GetMethods(BindingFlags.Public | BindingFlags.Instance);
                }
                else if(invokeData.MethodType == MethodType.Static) {
                    methodInfos = types[typeIndex].GetMethods(BindingFlags.Public | BindingFlags.Static);
                }

                foreach(MethodInfo methodInfo in methodInfos) {
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
                    if(parameters.Length == 0) _sb.Append("void");
                    _sb.Append(")");
                    string menuLabel = _sb.ToString();
                    _sb.Clear();

                    methodMenu.AddItem(
                        new GUIContent(menuLabel),
                        false,
                        () => {
                            Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                            invokeData.MethodData = new MethodData(methodInfo, _converters);
                        }
                    );
                }

                methodMenu.ShowAsContext();
            }

            if(string.IsNullOrEmpty(invokeData.MethodData.MethodName) == false) {
                Rect rect6 = CutSingleLineRect(ref position);

                MethodInfo methodInfo = invokeData.TypeId.ResolveType()
                    .GetMethod(
                        invokeData.MethodData.MethodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                        null,
                        invokeData.MethodData.ArgData.Select(x => x.TypeId.ResolveType()).ToArray(),
                        null);

                _sb.Append(TypeNameUtil.ConvertToPrimitiveTypeName(invokeData.MethodData.ReturnTypeId.TypeName));
                _sb.Append(" ");
                _sb.Append(invokeData.MethodData.MethodName);
                _sb.Append("(");
                IEnumerable<MethodArgData> argData = invokeData.MethodData.ArgData;
                for(int i = 0; i < argData.Count(); i++) {
                    if(i != 0) _sb.Append(", ");
                    _sb.Append(TypeNameUtil.ConvertToPrimitiveTypeName(argData.ElementAt(i).TypeId.TypeName));
                    _sb.Append(" ");
                    _sb.Append(argData.ElementAt(i).ArgName);
                }
                if(argData.Count() == 0) _sb.Append("void");
                _sb.Append(")");
                string methodLabel = _sb.ToString();
                _sb.Clear();

                EditorGUI.LabelField(rect6, methodLabel);

                int methodInfoIndex = 0;
                foreach(MethodArgData data in invokeData.MethodData.ArgData) {
                    Rect dataRect = CutSingleLineRect(ref position);
                    List<Rect> dataRects = RectUtil.DivideRectHorizontal(dataRect, _argRowWidths, 0, 0);

                    Type argType = data.TypeId.ResolveType();
                    ValueStringConverterBase converter = _converters.FindConverter(argType);

                    ParameterInfo parameterInfo = methodInfo.GetParameters()[methodInfoIndex++];

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
                        Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                        data.ResolveWay = newResolveWay;
                    }

                    if(data.ResolveWay == MethodArgResolveWay.Value) {
                        if(converter == null) {
                            EditorGUI.LabelField(dataRects[3], "----");
                        }
                        else {
                            string newValueString = converter.DrawField(dataRects[3], "", data.ValueString);
                            if(newValueString != data.ValueString) {
                                Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                                data.ValueString = newValueString;
                            }
                        }
                    }
                    else if(data.ResolveWay == MethodArgResolveWay.Variable) {
                        IEnumerable<VariableBase> variables = command.GetAvailableVariableDefines(parameterInfo.ParameterType).ToArray();
                        string[] variableNames = variables.Select(x => x.Name).ToArray();
                        string[] variableIds = variables.Select(x => x.Id).ToArray();
                        int variableIdIndex = Array.IndexOf(variableIds, data.VaribaleId);
                        variableIdIndex = EditorGUI.Popup(dataRects[3], variableIdIndex, variableNames);
                        string newVariableId = "";
                        if(variableIdIndex > -1) {
                            newVariableId = variableIds[variableIdIndex];
                        }
                        if(newVariableId != data.VaribaleId) {
                            Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                            data.VaribaleId = newVariableId;
                        }
                    }
                }

                Rect returnValueLabelRect = CutSingleLineRect(ref position);
                EditorGUI.LabelField(returnValueLabelRect, "Return Value (" + TypeNameUtil.ConvertToPrimitiveTypeName(methodInfo.ReturnType.Name) + ")");

                Rect returnValueHandlingEditRect = CutSingleLineRect(ref position);
                List<Rect> returnValueHandlingEditRects = RectUtil.DivideRectHorizontal(returnValueHandlingEditRect, new RectUtil.LayoutLength[] {new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1)});
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
                    Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                    invokeData.MethodData.ReturnValueHandling = newReturnValueHandling;
                }

                if(invokeData.MethodData.ReturnValueHandling == ReturnValueHandling.SetToVariable) {
                    IEnumerable<VariableBase> variables = command.GetAvailableVariableDefines(methodInfo.ReturnType).ToArray();
                    string[] variableNames = variables.Select(x => x.Name).ToArray();
                    string[] variableIds = variables.Select(x => x.Id).ToArray();
                    int variableIdIndex = Array.IndexOf(variableIds, invokeData.MethodData.ReturnValueReceiveVariableId);
                    variableIdIndex = EditorGUI.Popup(returnValueHandlingEditRects[1], variableIdIndex, variableNames);
                    string newVariableId = "";
                    if(variableIdIndex > -1) {
                        newVariableId = variableIds[variableIdIndex];
                    }
                    if(newVariableId != invokeData.MethodData.ReturnValueReceiveVariableId) {
                        Undo.RecordObject(property.serializedObject.targetObject, "ReflectionMethodInvokeData changed");
                        invokeData.MethodData.ReturnValueReceiveVariableId = newVariableId;
                    }
                }
            }
        }

        public Rect CutSingleLineRect(ref Rect rect) {
            Rect ret = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return ret;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            ReflectionMethodInvokeData invokeData = property.GetObject() as ReflectionMethodInvokeData;
            float height = 0;
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
            if(invokeData.MethodType == MethodType.Instance) height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if(string.IsNullOrEmpty(invokeData.MethodData.MethodName) == false) {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
                foreach(MethodArgData argData in invokeData.MethodData.ArgData) {
                    height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                }
            }
            return height;
        }
    }
}