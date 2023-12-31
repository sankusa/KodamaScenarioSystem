using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(CallScenarioCommand))]
    public class CallScenarioCommandDrawer : PropertyDrawer {
        private static string[] _emptyPageNames = new string[]{"<Default>"};
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // CallType
            SerializedProperty callTypeProp = property.FindPropertyRelative("_callType");

            Rect callTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(callTypeRect, callTypeProp);

            // TargetScenario
            SerializedProperty targetScenarioProp = property.FindPropertyRelative("_targetScenario");

            Rect targetScenarioRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            position.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(targetScenarioRect, targetScenarioProp);

            // 更新後の値を取得
            Scenario targetScenario = targetScenarioProp.objectReferenceValue as Scenario;

            // TargetPage
            SerializedProperty targetPageProp = property.FindPropertyRelative("_targetPage");
            ScenarioPage targetPage = targetPageProp.objectReferenceValue as ScenarioPage;

            if(targetScenario == null) {
                targetPageProp.objectReferenceValue = null;
            }
            else {
                Rect targetPageRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                string[] pageNames = _emptyPageNames.Concat(targetScenario.Pages.Select(x => x.name)).ToArray();
                int selectedIndex = Array.IndexOf(pageNames, targetPage?.name);
                if(selectedIndex == -1) selectedIndex = 0;
                
                selectedIndex = EditorGUI.Popup(targetPageRect, "Target Page", selectedIndex, pageNames);

                if(selectedIndex == 0) {
                    targetPageProp.objectReferenceValue = null;
                }
                else if(0 <= selectedIndex) {
                    ScenarioPage newTargetPage = targetScenario.FindPageByName(pageNames[selectedIndex]);
                    targetPageProp.objectReferenceValue = newTargetPage;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}