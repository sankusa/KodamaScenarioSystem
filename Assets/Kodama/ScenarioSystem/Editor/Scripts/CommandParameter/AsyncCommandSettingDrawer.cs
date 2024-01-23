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

            property.isExpanded = HeaderFoldOut.DrawFoldoutGroupFrame(new Rect(rect.x, rect.y, rect.width, rect.height), label.text, property.isExpanded);

            if(property.isExpanded) {
                rect.yMin += HeaderFoldOut.HeaderHeight + EditorGUIUtility.standardVerticalSpacing;

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
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.isExpanded == false) return HeaderFoldOut.HeaderHeight;
            SerializedProperty waitProp = property.FindPropertyRelative("_wait");
            return HeaderFoldOut.HeaderHeight + EditorGUIUtility.standardVerticalSpacing
                + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                + (waitProp.boolValue == false ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0);
        }
    }
}