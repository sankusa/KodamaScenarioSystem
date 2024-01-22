using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(AsyncCommandSetting), true)]
    public class AsyncCommandSettingDrawer : PropertyDrawer {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            SerializedProperty waitProp = property.FindPropertyRelative("_wait");
            SerializedProperty returnValueSetTargetProp = property.FindPropertyRelative("_setUniTaskTo");
            AsyncCommandSetting setting = property.GetObject() as AsyncCommandSetting;

            rect.yMax -= 6;
            using (new BackgroundColorScope(new Color(1, 1, 1, 0.5f))) {
                GUI.Box(new Rect(rect) {xMin = rect.xMin - 4, xMax = rect.xMax + 4}, "", GUIStyles.AsyncCommandSettingBox);
            }

            Rect labelRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
            rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(labelRect, nameof(AsyncCommandSetting), EditorStyles.boldLabel);

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