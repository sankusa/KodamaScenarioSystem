#if KODAMA_SCENARIO_LOCALIZATION_SUPPORT
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(LocalizedText))]
    public class LocalizedTextDrawer : PropertyDrawer {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            Locale projectLocale = LocalizationSettings.ProjectLocale;
            if(projectLocale == null) {
                return;
            }
            SerializedProperty recordsProp = property.FindPropertyRelative("_records");
            SerializedProperty targetRecordProp = null;
            for(int i = 0; i < recordsProp.arraySize; i++) {
                SerializedProperty currentRecordProp = recordsProp.GetArrayElementAtIndex(i);
                SerializedProperty localeCodeProp = currentRecordProp.FindPropertyRelative("_localeCode");
                if(localeCodeProp.stringValue == projectLocale.Identifier.Code) {
                    targetRecordProp = currentRecordProp;
                    break;
                }
            }
            
            if(targetRecordProp != null) {
                SerializedProperty textProp = targetRecordProp.FindPropertyRelative("_text");
                EditorGUI.PropertyField(rect, textProp, new GUIContent($"{label.text} ({projectLocale.Identifier.Code})"));
            }
            else {
                EditorGUI.LabelField(rect, "No record exists for selected record");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            Locale projectLocale = LocalizationSettings.ProjectLocale;
            if(projectLocale == null) {
                return EditorGUIUtility.singleLineHeight;
            }
            SerializedProperty recordsProp = property.FindPropertyRelative("_records");
            SerializedProperty targetRecordProp = null;
            for(int i = 0; i < recordsProp.arraySize; i++) {
                SerializedProperty currentRecordProp = recordsProp.GetArrayElementAtIndex(i);
                SerializedProperty localeCodeProp = currentRecordProp.FindPropertyRelative("_localeCode");
                if(localeCodeProp.stringValue == projectLocale.Identifier.Code) {
                    targetRecordProp = currentRecordProp;
                    break;
                }
            }

            if(targetRecordProp != null) {
                SerializedProperty textProp = targetRecordProp.FindPropertyRelative("_text");
                return EditorGUI.GetPropertyHeight(textProp);
            }
            else {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
#endif