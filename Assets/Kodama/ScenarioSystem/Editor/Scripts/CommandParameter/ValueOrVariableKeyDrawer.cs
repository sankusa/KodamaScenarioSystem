using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(ValueOrVariableKey<>), true)]
    public class ValueOrVariableKeyDrawer : PropertyDrawer {
        private const float _miniPopUpWidth = 20;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            ValueOrVariableKey valueOrVariableKey = property.GetObject() as ValueOrVariableKey;
            Type targetType = valueOrVariableKey.TargetType;
            CommandBase command = property.serializedObject.targetObject as CommandBase;
            SerializedProperty variableKeyProp = property.FindPropertyRelative("_variableKey");

            if(!string.IsNullOrEmpty(label.text)) {
                Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
                EditorGUI.LabelField(labelRect, label);
                rect.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
            }

            int originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if(valueOrVariableKey.HasKey() == false) {
                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                Rect valueFieldRect = new Rect(rect.x, rect.y, rect.width - _miniPopUpWidth - 2, rect.height);

                // シリアライズ不可能な型
                if(valueProp == null) {
                    EditorGUI.LabelField(valueFieldRect, "Default");
                }
                else {
                    EditorGUI.PropertyField(valueFieldRect, valueProp, GUIContent.none);
                }
                rect.xMin += rect.width - _miniPopUpWidth;
            }
        
            EditorGUI.PropertyField(rect, variableKeyProp, GUIContent.none);

            EditorGUI.indentLevel= originalIndentLevel;
        }
    }
}