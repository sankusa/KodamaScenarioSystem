using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(ValueOrVariableName<>), true)]
    public class ValueOrVariableNameDrawer : PropertyDrawer {
        private const string _emptyString = "<Value>";
        private const float _miniPopUpWidth = 20;
        private static readonly string[] _emptyVariableNameArray = new string[]{_emptyString};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            ValueOrVariableName valueOrVariableName = property.GetObject() as ValueOrVariableName;
            Type targetType = valueOrVariableName.TargetType;
            ScenarioPage page = property.serializedObject.targetObject as ScenarioPage;
            Scenario scenario = page.Scenario;
            SerializedProperty variableNameProp = property.FindPropertyRelative("_variableName");

            if(!string.IsNullOrEmpty(label.text)) {
                Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
                EditorGUI.LabelField(labelRect, label);
                rect.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
            }

            if(string.IsNullOrEmpty(variableNameProp.stringValue)) {
                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                Rect valueFieldRect = new Rect(rect.x, rect.y, rect.width - _miniPopUpWidth - 2, rect.height);

                if(valueProp == null) {
                    EditorGUI.LabelField(valueFieldRect, "Default");
                }
                else {
                    EditorGUI.PropertyField(valueFieldRect, valueProp, GUIContent.none);
                }
                rect.xMin += rect.width - _miniPopUpWidth;
            }
        
            string emptyConvertedVariableName = string.IsNullOrEmpty(variableNameProp.stringValue) ? _emptyString : variableNameProp.stringValue;
            string[] variableNames = scenario.Variables.Where(x => x.TargetType == targetType).Select(x => x.Name).ToArray();
            string[] variableNamesAndEmpty = _emptyVariableNameArray.Concat(variableNames).ToArray();

            int selectedIndex = Array.IndexOf(variableNamesAndEmpty, emptyConvertedVariableName);

            selectedIndex = EditorGUI.Popup(rect, selectedIndex, variableNamesAndEmpty);

            if(selectedIndex == -1) selectedIndex = 0;
            variableNameProp.stringValue = selectedIndex == 0 ? "" : variableNamesAndEmpty[selectedIndex];
        }
    }
}