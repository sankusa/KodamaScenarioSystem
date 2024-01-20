using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(VariableName<>), true)]
    public class VariableNameDrawer : PropertyDrawer {
        private static readonly string[] _emptyVariableNameArray = new string[]{Labels.Label_Empty};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            VariableName variableName = property.GetObject() as VariableName;
            Type targetType = variableName.TargetType;
            CommandBase command = property.serializedObject.targetObject as CommandBase;
            ScenarioPage page = command.ParentPage;
            Scenario scenario = page.ParentScenario;
            SerializedProperty nameProp = property.FindPropertyRelative("_name");
        
            string emptyConvertedVariableName = string.IsNullOrEmpty(nameProp.stringValue) ? Labels.Label_Empty : nameProp.stringValue;
            string[] variableNames = scenario.Variables.Where(x => x.TargetType == targetType).Select(x => x.Name).ToArray();
            string[] variableNamesAndEmpty = _emptyVariableNameArray.Concat(variableNames).ToArray();

            int selectedIndex = Array.IndexOf(variableNamesAndEmpty, emptyConvertedVariableName);
            
            selectedIndex = EditorGUI.Popup(rect, label.text, selectedIndex, variableNamesAndEmpty);
            
            if(selectedIndex == -1) selectedIndex = 0;
            nameProp.stringValue = selectedIndex == 0 ? "" : variableNamesAndEmpty[selectedIndex];
        }
    }
}