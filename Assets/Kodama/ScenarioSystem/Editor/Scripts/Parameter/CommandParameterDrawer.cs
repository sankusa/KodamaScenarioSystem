using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(CommandParameter<>))]
    public class CommandParameterDrawer : PropertyDrawer {
        private const string _emptyString = "<Value>";
        private const float _miniPopUpWidth = 34;
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            SerializedProperty variableNameProp = property.FindPropertyRelative("_variableName");

            Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, label);
            
            Rect popUpRect;
            if(string.IsNullOrEmpty(variableNameProp.stringValue)) {
                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                Rect valueFieldRect = new Rect(rect.x + labelRect.width, rect.y, rect.width - labelRect.width - _miniPopUpWidth, rect.height);

                EditorGUI.PropertyField(valueFieldRect, valueProp, GUIContent.none);
                popUpRect = new Rect(rect.xMax - _miniPopUpWidth, rect.y, _miniPopUpWidth, rect.height);
            }
            else {
                popUpRect = new Rect(rect.x + labelRect.width, rect.y, rect.width - labelRect.width, rect.height);
            }
        
            string emptyConvertedVariableName = string.IsNullOrEmpty(variableNameProp.stringValue) ? _emptyString : variableNameProp.stringValue;
            Type targetType = fieldInfo.FieldType.GetField("_value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FieldType;
            string[] variableNames = ((Scenario)property.serializedObject.targetObject).Variables.Where(x => x.GetType().BaseType.GenericTypeArguments[0] == targetType).Select(x => x.Name).ToArray();
            string[] empty = new string[]{_emptyString};
            string[] variableNamesAndEmpty = empty.Concat(variableNames).ToArray();

            int selectedIndex = Array.IndexOf(variableNamesAndEmpty, variableNameProp.stringValue);
            selectedIndex = EditorGUI.Popup(popUpRect, selectedIndex, variableNamesAndEmpty);
            if(selectedIndex == -1) selectedIndex = 0;
            variableNameProp.stringValue = variableNamesAndEmpty[selectedIndex] == _emptyString ? "" : variableNamesAndEmpty[selectedIndex];
        
        }
    }
}