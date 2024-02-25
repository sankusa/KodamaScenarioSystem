#if KODAMA_SCENARIO_LOCALIZATION_SUPPORT
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Kodama.ScenarioSystem.Editor {
    public class ScenarioLocalizedTextManagementWindow : EditorWindow {
        private Scenario _scenario;
        private ScenarioPage _page;
        private Vector2 _scrollPos;
        private float _columnWidth = 220;

        public static void Open(Scenario scenario) {
            var window = GetWindow<ScenarioLocalizedTextManagementWindow>("Localized Text Management");
            window._scenario = scenario;
            window._page = null;
        }

        void OnEnable() {

        }

        void OnGUI() {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Scenario", _scenario, typeof(Scenario), false);
            EditorGUI.EndDisabledGroup();

            if(_page == null && _scenario.Pages.Count > 0) {
                _page = _scenario.DefaultPage;
            }

            int pageIndex = _page != null ? _page.Index : -1;
            pageIndex = EditorGUILayout.Popup("Page", pageIndex, _scenario.Pages.Select(x => x.name).ToArray());
            if(pageIndex >= 0) {
                _page = _scenario.Pages[pageIndex];
            }

            _columnWidth = EditorGUILayout.Slider("Column Width", _columnWidth, 100, 800);

            EditorGUILayout.BeginHorizontal();
            GUILayoutUtility.GetRect(0, 0, GUILayout.Width(_columnWidth));
            for(int i = 0; i < _scenario.LocaleCodes.Count; i++) {
                EditorGUILayout.Space(3, false);

                Rect rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight, GUILayout.Width(_columnWidth));
                string localeCode = _scenario.LocaleCodes[i];
                EditorGUI.LabelField(new Rect(rect) {xMax = rect.xMax - 20}, localeCode, GUIStyles.LeanGroupBox);
                if(GUI.Button(new Rect(rect) {xMin = rect.xMax - 20}, CommonEditorResources.Instance.CommandDeleteIcon)) {
                    _scenario.RemoveLocaleCode(localeCode);
                }
            }
            EditorGUILayout.EndHorizontal();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            if(_page != null) {
                foreach(CommandBase command in _page.Commands) {
                    SerializedObject serializedCommand = new SerializedObject(command);

                    serializedCommand.Update();

                    SerializedProperty property = serializedCommand.GetIterator();
                    while(property.NextVisible(true)) {
                        if(property.type != nameof(LocalizedText)) continue;

                        SerializedProperty recordsProp = property.FindPropertyRelative("_records");

                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextArea(command.Index + " : " + command.GetType().Name, GUIStyles.LeanGroupBox, GUILayout.ExpandHeight(true), GUILayout.Width(_columnWidth));
                        EditorGUI.EndDisabledGroup();

                        foreach(string localeCode in _scenario.LocaleCodes) {
                            EditorGUILayout.Space(3, false);

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
                                Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandHeight(true), GUILayout.Width(_columnWidth));
                                EditorGUI.LabelField(rect, "None");
                                if(GUI.Button(new Rect(rect) {xMin = rect.xMax - 20}, CommonEditorResources.Instance.CommandAddIcon)) {
                                    LocalizedText localizedText = property.GetObject() as LocalizedText;
                                    localizedText.AddRecord(command, localeCode);
                                }
                            }
                            else {
                                SerializedProperty textProp = targetRecordProp.FindPropertyRelative("_text");
                                float height = GUI.skin.textArea.CalcHeight(new GUIContent(textProp.stringValue), _columnWidth - 20);
                                Rect rect = GUILayoutUtility.GetRect(0, height + 8, GUILayout.ExpandHeight(true), GUILayout.Width(_columnWidth));
                                textProp.stringValue = EditorGUI.TextArea(new Rect(rect) {xMax = rect.xMax - 20}, textProp.stringValue);
                                if(GUI.Button(new Rect(rect) {xMin = rect.xMax - 20}, CommonEditorResources.Instance.CommandDeleteIcon)) {
                                    LocalizedText localizedText = property.GetObject() as LocalizedText;
                                    localizedText.RemoveRecord(command, localeCode);
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    serializedCommand.ApplyModifiedProperties();
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("Add Locale")) {
                    GenericMenu addLocaleMenu = new GenericMenu();
                    foreach(string nonAddedLocaleCode in LocalizationSettings.AvailableLocales.Locales.Select(x => x.Identifier.Code)
                        .Where(x => _scenario.LocaleCodes.Contains(x) == false)) {
                        addLocaleMenu.AddItem(new GUIContent(nonAddedLocaleCode), false, () => {
                            _scenario.AddLocaleCode(nonAddedLocaleCode);
                        });
                    }
                    addLocaleMenu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
            }

            if(Event.current.type == EventType.KeyUp) {
                Repaint();
            }
        }
    }
}
#endif