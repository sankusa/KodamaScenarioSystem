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
            CommandBase command = property.serializedObject.targetObject as CommandBase;

            // 型変更時の各値の更新とSerializedPropertyを通した値の更新が同フレームで行われた場合に
            // 変更が正しく反映されない事象が発生したので、型変更時には他の更新は行わない。
            bool typeChangedThisFrame = false;

            Rect headerRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel++;

            // 型選択
            Rect typeRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            int typeIndex = Array.IndexOf(_variableTypeNames, TypeNameUtil.ConvertToPrimitiveTypeName(targetType.Name));
            int newTypeIndex = EditorGUI.Popup(typeRect, "Type", typeIndex, _variableTypeNames);
            if(newTypeIndex != typeIndex) {
                targetType = _availableTypes[newTypeIndex];
                Undo.RecordObject(command, "Condition Type Chenged");
                condition.VariableName = Activator.CreateInstance(_variableNameDic[targetType]) as VariableName;
                condition.ValueOrVariableName = Activator.CreateInstance(_valueOrVariableNameDic[targetType]) as ValueOrVariableName;
                condition.Operator = Condition.CompareOperator.EqualTo;
                typeChangedThisFrame = true;
            }
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // 変数1
            Rect variableNameRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            if(typeChangedThisFrame == false && variableNameProp != null) {
                EditorGUI.PropertyField(variableNameRect, variableNameProp, new GUIContent("A"), true);
            }
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            // 比較演算子
            Rect operatorRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            Condition.CompareOperator[] operators;
            if(targetType.GetInterfaces().Any(t => t == typeof(IComparable))) {
                operators = Condition.OperatorsForCompareable;
            }
            else {
                operators = Condition.OperatorsForNotCompareable;
            }
            int operatorIndex = Array.IndexOf(operators, condition.Operator);
            if(operatorIndex == -1) operatorIndex = 0;
            operatorIndex = EditorGUI.Popup(operatorRect, "Operator", operatorIndex, operators.Select(x => x.GetOperatorString()).ToArray());
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if(typeChangedThisFrame == false) {
                operatorProp.enumValueIndex = (int)operators[operatorIndex];
            }

            // 変数2(or値)
            Rect valueOrVariableRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            if(typeChangedThisFrame == false && valueOrVariableNameProp != null) {
                EditorGUI.PropertyField(valueOrVariableRect, valueOrVariableNameProp, new GUIContent("B"), true);
            }
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
        }
    }


}