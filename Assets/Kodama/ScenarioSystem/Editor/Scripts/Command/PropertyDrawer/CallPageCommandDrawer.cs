using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(CallPageCommand))]
    public class CallPageCommandDrawer : UnityEditor.Editor {
        private static string[] _emptyPageNames = new string[]{"<Default>"};
        public override void OnInspectorGUI() {
            // CallType
            SerializedProperty callTypeProp = serializedObject.FindProperty("_callType");
            EditorGUILayout.PropertyField(callTypeProp);

            // TargetPage
            SerializedProperty targetPageProp = serializedObject.FindProperty("_targetPage");
            SerializedProperty pageProp = serializedObject.FindProperty("_page");
            ScenarioPage targetPage = targetPageProp.objectReferenceValue as ScenarioPage;
            ScenarioPage page = pageProp.objectReferenceValue as ScenarioPage;

            string[] pageNames = _emptyPageNames.Concat(page.Scenario.Pages.Select(x => x.name)).ToArray();
            int selectedIndex = Array.IndexOf(pageNames, targetPage?.name);
            if(selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Target Page", selectedIndex, pageNames);
            
            if(selectedIndex == 0) {
                targetPageProp.objectReferenceValue = null;
            }
            else if(0 <= selectedIndex) {
                ScenarioPage newTargetPage = page.Scenario.FindPageByName(pageNames[selectedIndex]);
                targetPageProp.objectReferenceValue = newTargetPage;
            }
        }
    }
}