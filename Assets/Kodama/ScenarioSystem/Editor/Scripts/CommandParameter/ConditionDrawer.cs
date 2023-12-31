using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(Condition))]
    public class ConditionDrawer : PropertyDrawer {
        private static RectUtil.LayoutLength[] _widths = new RectUtil.LayoutLength[]{
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(1),
            new RectUtil.LayoutLength(1)
        };

        private static List<Type> _availableTypes;
        private static string[] _variableTypeNames;
        private static Dictionary<Type, Type> _variableNameDic = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> _valueOrVariableNameDic = new Dictionary<Type, Type>();

        static ConditionDrawer() {
            IEnumerable<Type> serializableVariableNames = TypeCache.GetTypesDerivedFrom<VariableName>().Where(x => x.IsAbstract == false && x.IsGenericType == false);
            foreach(Type serializableVariableName in serializableVariableNames) {
                Type genericTypeArg = serializableVariableName.BaseType.GenericTypeArguments[0];
                _variableNameDic[genericTypeArg] = serializableVariableName;
            }

            IEnumerable<Type> serializableValueOrVariableNames = TypeCache.GetTypesDerivedFrom<ValueOrVariableName>().Where(x => x.IsAbstract == false && x.IsGenericType == false);
            foreach(Type serializableValueOrVariableName in serializableValueOrVariableNames) {
                Type genericTypeArg = serializableValueOrVariableName.BaseType.GenericTypeArguments[0];
                _valueOrVariableNameDic[genericTypeArg] = serializableValueOrVariableName;
            }

            _availableTypes = _valueOrVariableNameDic.Keys.ToList();
            _variableTypeNames = _availableTypes.Select(x => TypeNameUtil.ConvertToPrimitiveTypeName(x.Name)).ToArray();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            SerializedProperty variableNameProp = property.FindPropertyRelative("_variableName");
            SerializedProperty operatorProp = property.FindPropertyRelative("_operator");
            SerializedProperty valueOrVariableNameProp = property.FindPropertyRelative("_valueOrVariableName");
            Condition condition = property.GetObject() as Condition;
            Type targetType = condition.VariableName.TargetType;
            Scenario scenario = property.serializedObject.targetObject as Scenario;

            // 型変更時の各値の更新とSerializedPropertyを通した値の更新が同フレームで行われた場合に
            // 変更が正しく反映されない事象が発生したので、型変更時には他の更新は行わない。
            bool typeChangedThisFrame = false;

            if(string.IsNullOrEmpty(label.text) == false) {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"<b>{label.text}</b>", GUIStyles.SummaryLabel);
                rect.yMin += EditorGUIUtility.singleLineHeight;
            }

            GUI.Box(rect, "");
            rect = RectUtil.Margin(rect, 4, 4, 4, 4);

            List<Rect> rects = RectUtil.DivideRectVertical(rect, _widths);
            // 型選択
            int typeIndex = Array.IndexOf(_variableTypeNames, TypeNameUtil.ConvertToPrimitiveTypeName(targetType.Name));
            int newTypeIndex = EditorGUI.Popup(rects[0], "Type", typeIndex, _variableTypeNames);
            if(newTypeIndex != typeIndex) {
                targetType = _availableTypes[newTypeIndex];
                Undo.RecordObject(scenario, "Condition Type Chenged");
                condition.VariableName = Activator.CreateInstance(_variableNameDic[targetType]) as VariableName;
                condition.ValueOrVariableName = Activator.CreateInstance(_valueOrVariableNameDic[targetType]) as ValueOrVariableName;
                condition.Operator = Condition.CompareOperator.EqualTo;
                EditorUtility.SetDirty(scenario);
                typeChangedThisFrame = true;
            }

            // 変数1
            if(typeChangedThisFrame == false && variableNameProp != null) {
                EditorGUI.PropertyField(rects[1], variableNameProp, new GUIContent("Variable Name"), true);
            }
            
            // 比較演算子
            Condition.CompareOperator[] operators;
            if(targetType.GetInterfaces().Any(t => t == typeof(IComparable))) {
                operators = Condition.OperatorsForCompareable;
            }
            else {
                operators = Condition.OperatorsForNotCompareable;
            }
            int operatorIndex = Array.IndexOf(operators, condition.Operator);
            if(operatorIndex == -1) operatorIndex = 0;
            operatorIndex = EditorGUI.Popup(rects[2], "Operator", operatorIndex, operators.Select(x => x.GetOperatorString()).ToArray());

            if(typeChangedThisFrame == false) {
                operatorProp.enumValueIndex = (int)operators[operatorIndex];
            }

            // 変数2(or値)
            if(typeChangedThisFrame == false && valueOrVariableNameProp != null) {
                EditorGUI.PropertyField(rects[3], valueOrVariableNameProp, new GUIContent("Value Or Variable Name"), true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 88 + (string.IsNullOrEmpty(label.text) ? 0 : EditorGUIUtility.singleLineHeight);
        }
    }


}