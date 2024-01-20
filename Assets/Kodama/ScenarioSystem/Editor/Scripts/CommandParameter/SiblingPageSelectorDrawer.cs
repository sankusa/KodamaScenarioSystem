using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(SiblingPageSelector))]
    public class SiblingPageSelectorDrawer : PropertyDrawer {
        private static string[] _emptyPageNames = new string[]{Labels.Label_DefaultPage};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            Rect headerRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel++;

            CommandBase command = property.serializedObject.targetObject as CommandBase;
            ScenarioPage parentPage = command.ParentPage;
            SerializedProperty pageProp = property.FindPropertyRelative("_page");
            ScenarioPage page = pageProp.objectReferenceValue as ScenarioPage;

            IEnumerable<ScenarioPage> siblingPages = parentPage.ParentScenario.Pages.OrderBy(x => x.name);
            int selectIndex = siblingPages.IndexOf(page);
            selectIndex++;

            string[] pageNames = _emptyPageNames
                .Concat(siblingPages.Select(x => x.name))
                .ToArray();

            selectIndex = EditorGUI.Popup(rect, "Page", selectIndex, pageNames);

            if(selectIndex == 0) {
                pageProp.objectReferenceValue = null;
            }
            else {
                ScenarioPage newTargetPage = siblingPages.ElementAt(selectIndex - 1);
                pageProp.objectReferenceValue = newTargetPage;
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}