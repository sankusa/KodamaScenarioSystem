using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(AsyncCommandWaitSetting), true)]
    public class AsyncCommandWaitSettingDrawer : PropertyDrawer {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            SerializedProperty waitProp = property.FindPropertyRelative("_wait");
            SerializedProperty returnValueSetTargetProp = property.FindPropertyRelative("_setUniTaskTo");
            AsyncCommandWaitSetting setting = property.GetObject() as AsyncCommandWaitSetting;

            rect.yMax -= 6;
            GUI.Box(new Rect(rect) {xMin = rect.xMin - 4, xMax = rect.xMax + 4}, "", GUIStyles.LeanGroupBox);

            Rect labelRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(labelRect, "Wait Setting", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            if(setting.Wait == true) {
                EditorGUI.PropertyField(rect, waitProp);
                SerializedProperty variableIdProp = returnValueSetTargetProp.FindPropertyRelative("_id");
                variableIdProp.stringValue = "";
            }
            else {
                Rect waitRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
                rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect returnValueTargetRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};

                EditorGUI.PropertyField(waitRect, waitProp);
                EditorGUI.PropertyField(returnValueTargetRect, returnValueSetTargetProp, new GUIContent("Set UniTask To"));
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty waitProp = property.FindPropertyRelative("_wait");
            return EditorGUIUtility.singleLineHeight * 2
                + EditorGUIUtility.standardVerticalSpacing * 2
                + (waitProp.boolValue == false ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0)
                + 6;
        }
    }
}