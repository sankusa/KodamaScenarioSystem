using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;
using System;
using System.Linq;
using UnityEditorInternal;
using System.Reflection;

namespace Kodama.ScenarioSystem.Editor {
    // SerializeReferenceのFindPropertyRelative及びPropertyFieldは遅いので、可能な限り使用しない
    [CustomPropertyDrawer(typeof(Variable<>), true)]
    public class VariableDrawer {
        private static RectUtil.LayoutLength[] _rectLengths = new RectUtil.LayoutLength[]{ new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1.5f)};

        private Dictionary<Type, VariableValueFieldBase> _customValueDrawerDic = new Dictionary<Type, VariableValueFieldBase>();

        public VariableDrawer() {
            var drawerTypes = TypeCache.GetTypesWithAttribute(typeof(CustomVariableValueFieldAttribute));
            foreach(Type drawerType in drawerTypes) {
                var attribute = drawerType.GetCustomAttribute<CustomVariableValueFieldAttribute>();
                _customValueDrawerDic[attribute.Type] = (VariableValueFieldBase)Activator.CreateInstance(drawerType);
            }
        }

        public void OnGUI(Rect rect, SerializedProperty property, VariableBase variable) {
            // 必要な値を取得
            Scenario scenario = property.serializedObject.targetObject as Scenario;
            FieldInfo valueFieldInfo = variable.GetType().BaseType.GetField(Variable<object>.VariableName_Value, BindingFlags.NonPublic | BindingFlags.Instance);
            string typeName = TypeNameUtil.ConvertToPrimitiveTypeName(valueFieldInfo.FieldType.Name);

            // 各要素の描画範囲
            Rect[] rects = RectUtil.DivideRectHorizontal(rect, _rectLengths);
            rects[0] = new Rect(rects[0].x, rects[0].y, rects[0].width, EditorGUIUtility.singleLineHeight);
            rects[1] = new Rect(rects[1].x, rects[1].y, rects[1].width - 4, EditorGUIUtility.singleLineHeight);

            // 描画
            EditorGUI.LabelField(rects[0], typeName);

            EditorGUI.BeginChangeCheck();
            string variableName = EditorGUI.DelayedTextField(rects[1], variable.Name);
            if(EditorGUI.EndChangeCheck() && scenario.Variables.FirstOrDefault(x => x.Name == variableName) == null) {
                Undo.RecordObject(scenario, "Change Variable Name");
                variable.Name = variableName;
                EditorUtility.SetDirty(scenario);
            }

            if(_customValueDrawerDic.ContainsKey(valueFieldInfo.FieldType)) {
                _customValueDrawerDic[valueFieldInfo.FieldType].Draw(RectUtil.Margin(rects[2], bottomMargin: 2), scenario, variable);
            } 
            else {
                SerializedProperty valueProp = property.FindPropertyRelative(Variable<object>.VariableName_Value);
                if(valueProp == null) {
                    EditorGUI.LabelField(rects[2], "Default");
                }
                else {
                    EditorGUI.PropertyField(rects[2], valueProp, GUIContent.none, true);
                }
            }
        }

        public float GetPropertyHeight(SerializedProperty property, VariableBase variable) {
            SerializedProperty valueProp = property.FindPropertyRelative(Variable<object>.VariableName_Value);
            FieldInfo valueFieldInfo = variable.GetType().BaseType.GetField(Variable<object>.VariableName_Value, BindingFlags.NonPublic | BindingFlags.Instance);

            if(_customValueDrawerDic.ContainsKey(valueFieldInfo.FieldType)) {
                return _customValueDrawerDic[valueFieldInfo.FieldType].GetHeight();
            }

            if(valueProp == null) return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(valueProp, true);
        }
    }
}