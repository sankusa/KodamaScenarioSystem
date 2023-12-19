using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(VariableName<>), true)]
    public class VariableNameDrawer : PropertyDrawer {
        private const string _emptyString = "<None>";
        private static readonly string[] _emptyVariableNameArray = new string[]{_emptyString};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            VariableName variableName = property.GetObject() as VariableName;
            Type targetType = variableName.TargetType;
            Scenario scenario = property.serializedObject.targetObject as Scenario;
            SerializedProperty nameProp = property.FindPropertyRelative("_name");
        
            string emptyConvertedVariableName = string.IsNullOrEmpty(nameProp.stringValue) ? _emptyString : nameProp.stringValue;
            string[] variableNames = scenario.Variables.Where(x => x.TargetType == targetType).Select(x => x.Name).ToArray();
            string[] variableNamesAndEmpty = _emptyVariableNameArray.Concat(variableNames).ToArray();

            int selectedIndex = Array.IndexOf(variableNamesAndEmpty, emptyConvertedVariableName);
            
            using (new ContentColorScope(string.IsNullOrEmpty(variableName.Name) ? new Color(1, 0.3f, 0.3f) : Color.white)) {
                selectedIndex = EditorGUI.Popup(rect, label.text, selectedIndex, variableNamesAndEmpty);
            }
            
            if(selectedIndex == -1) selectedIndex = 0;
            nameProp.stringValue = selectedIndex == 0 ? "" : variableNamesAndEmpty[selectedIndex];
        }
    }
}