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
            // CallType
            SerializedProperty callTypeProp = property.FindPropertyRelative("_callType");

            Rect callTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(callTypeRect, callTypeProp);

            // TargetPage
            SerializedProperty targetPageProp = property.FindPropertyRelative("_targetPage");
            SerializedProperty pageProp = property.FindPropertyRelative("_page");
            ScenarioPage targetPage = targetPageProp.objectReferenceValue as ScenarioPage;
            ScenarioPage page = pageProp.objectReferenceValue as ScenarioPage;

            Rect targetPageRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            string[] pageNames = _emptyPageNames.Concat(page.Scenario.Pages.Select(x => x.name)).ToArray();
            int selectedIndex = Array.IndexOf(pageNames, targetPage?.name);
            if(selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(targetPageRect, "Target Page", selectedIndex, pageNames);
            
            if(selectedIndex == 0) {
                targetPageProp.objectReferenceValue = null;
            }
            else if(0 <= selectedIndex) {
                ScenarioPage newTargetPage = page.Scenario.FindPageByName(pageNames[selectedIndex]);
                targetPageProp.objectReferenceValue = newTargetPage;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}