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
        private static RectUtil.LayoutLength[] rectLengths = new RectUtil.LayoutLength[]{ new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1.5f)};
        private static readonly string _valueFieldName = "_value";

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
            FieldInfo valueFieldInfo = variable.GetType().BaseType.GetField(_valueFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            string typeName = TypeNameUtil.ConvertToPrimitiveTypeName(valueFieldInfo.FieldType.Name);

            // 各要素の描画範囲
            List<Rect> rects = RectUtil.DivideRectHorizontal(rect, rectLengths);
            rects[0] = new Rect(rects[0].x, rects[0].y, rects[0].width, EditorGUIUtility.singleLineHeight);
            rects[1] = new Rect(rects[1].x, rects[1].y, rects[1].width - 4, EditorGUIUtility.singleLineHeight);

            // 描画
            EditorGUI.LabelField(rects[0], typeName);

            EditorGUI.BeginChangeCheck();
            string variableName = EditorGUI.TextField(rects[1], variable.Name);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(scenario, "Change Variable Name");
                variable.Name = variableName;
                EditorUtility.SetDirty(scenario);
            }

            if(_customValueDrawerDic.ContainsKey(valueFieldInfo.FieldType)) {
                _customValueDrawerDic[valueFieldInfo.FieldType].Draw(RectUtil.Margin(rects[2], bottomMargin: 2), scenario, variable);
            } 
            else {
                SerializedProperty valueProp = property.FindPropertyRelative(_valueFieldName);
                EditorGUI.PropertyField(rects[2], valueProp, GUIContent.none, true);
            }
        }

        public float GetPropertyHeight(SerializedProperty property, VariableBase variable) {
            SerializedProperty valueProp = property.FindPropertyRelative(_valueFieldName);
            FieldInfo valueFieldInfo = variable.GetType().BaseType.GetField(_valueFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if(_customValueDrawerDic.ContainsKey(valueFieldInfo.FieldType)) {
                return _customValueDrawerDic[valueFieldInfo.FieldType].GetHeight();
            }
            else {
                return EditorGUI.GetPropertyHeight(valueProp, true);
            }
        }
    }
}