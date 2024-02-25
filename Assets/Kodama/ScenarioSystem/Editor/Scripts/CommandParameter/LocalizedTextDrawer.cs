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

            Rect modalWindowButtonRect = new Rect(rect.xMax - 48, rect.y, 48, 19);
            
            if(targetRecordProp != null) {
                SerializedProperty textProp = targetRecordProp.FindPropertyRelative("_text");
                EditorGUI.PropertyField(rect, textProp, new GUIContent($"{label.text} ({projectLocale.Identifier.Code})"));
            }
            else {
                Rect rect1 = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
                rect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect rect2 = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};

                EditorGUI.LabelField(rect1, label);
                EditorGUI.LabelField(rect2, "No record exists for project locale", GUIStyles.LeanGroupBox);
            }

            if(GUI.Button(modalWindowButtonRect, "Detail")) {
                var modalWindow = EditorWindow.CreateInstance(typeof(LocalizedTextEditModalWindow)) as LocalizedTextEditModalWindow;
                modalWindow.LocalizedTextProp = property;
                modalWindow.ShowAuxWindow();
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
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }

    public class LocalizedTextEditModalWindow : EditorWindow {
        public SerializedProperty LocalizedTextProp {private get; set;}

        void OnGUI() {
            SerializedProperty recordsProp = LocalizedTextProp.FindPropertyRelative("_records");
            CommandBase command = LocalizedTextProp.serializedObject.targetObject as CommandBase;
            Scenario scenario = command.ParentPage.ParentScenario;

            LocalizedTextProp.serializedObject.Update();

            foreach(string localeCode in scenario.LocaleCodes) {
                SerializedProperty targetRecordProp = null;
                for(int i = 0; i < recordsProp.arraySize; i++) {
                    SerializedProperty recordProp = recordsProp.GetArrayElementAtIndex(i);
                    SerializedProperty localeCodeProp = recordProp.FindPropertyRelative("_localeCode");
                    if(localeCodeProp.stringValue == localeCode) {
                        targetRecordProp = recordProp;
                        break;
                    }
                }

                if(targetRecordProp == null) {
                    EditorGUILayout.LabelField(localeCode + " : None");
                    Rect addButtonRect = new Rect(GUILayoutUtility.GetLastRect());
                    addButtonRect.xMin = addButtonRect.xMax - 19;
                    addButtonRect.yMax = addButtonRect.yMin + 19;
                    if(GUI.Button(addButtonRect, CommonEditorResources.Instance.CommandAddIcon)) {
                        LocalizedText localizedText = LocalizedTextProp.GetObject() as LocalizedText;
                        localizedText.AddRecord(command, localeCode);
                        Repaint();
                    }
                }
                else {
                    SerializedProperty textProp = targetRecordProp.FindPropertyRelative("_text");
                    EditorGUILayout.PropertyField(textProp, new GUIContent(localeCode));
                    //float height = GUI.skin.textArea.CalcHeight(new GUIContent(textProp.stringValue), _columnWidth - 20);
                    //Rect rect = GUILayoutUtility.GetRect(0, height + 8, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                    //textProp.stringValue = EditorGUI.TextArea(new Rect(rect) {xMax = rect.xMax - 20}, textProp.stringValue);
                    Rect deleteButtonRect = new Rect(GUILayoutUtility.GetLastRect());
                    deleteButtonRect.xMin = deleteButtonRect.xMax - 19;
                    deleteButtonRect.yMax = deleteButtonRect.yMin + 19;
                    if(GUI.Button(deleteButtonRect, CommonEditorResources.Instance.CommandDeleteIcon)) {
                        LocalizedText localizedText = LocalizedTextProp.GetObject() as LocalizedText;
                        localizedText.RemoveRecord(command, localeCode);
                        Repaint();
                    }
                }

                GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
            }

            LocalizedTextProp.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif