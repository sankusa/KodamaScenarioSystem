using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(CallPageCommand))]
    public class CallPageCommandDrawer : PropertyDrawer {
        private static string[] _emptyPageNames = new string[]{"<Default>"};
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CallPageCommand callPageCommand = property.GetObject() as CallPageCommand;

            Rect callTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty callTypeProp = property.FindPropertyRelative("_callType");
            EditorGUI.PropertyField(callTypeRect, callTypeProp);

            Rect targetPageRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty targetPageProp = property.FindPropertyRelative("_targetPage");
            
            string[] pageNames = _emptyPageNames.Concat(callPageCommand.Page.Scenario.Pages.Select(x => x.name)).ToArray();
            int selectedIndex = Array.IndexOf(pageNames, callPageCommand.TargetPage?.name);
            if(selectedIndex == -1) selectedIndex = 0;
            selectedIndex = EditorGUI.Popup(targetPageRect, "Target Page", selectedIndex, pageNames);
            if(selectedIndex == 0) {
                targetPageProp.objectReferenceValue = null;
            }
            else if(0 <= selectedIndex) {
                ScenarioPage newTargetPage = callPageCommand.Page.Scenario.Pages.FirstOrDefault(x => x.name == pageNames[selectedIndex]);
                targetPageProp.objectReferenceValue = newTargetPage;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}