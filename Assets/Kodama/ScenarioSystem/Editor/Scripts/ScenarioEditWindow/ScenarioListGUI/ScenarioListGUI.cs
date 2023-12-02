using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioListGUI {
        private SearchField _searchField;
        private ScenarioInfoTreeView _scenarioInfoTreeView;
        // 検索文字列のSesstionState用キー
        private static readonly string _searchStringSessionStateKey = $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(ScenarioListGUI)}_SearchString";

        public ScenarioListGUI() {
            // TreeView構築
            TreeViewState treeViewState = new TreeViewState();

            MultiColumnHeaderState.Column nameColumn = new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Name"),
                width = 200,
                minWidth = 200,
                autoResize = true,
            };

            MultiColumnHeaderState.Column pathColumn = new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Path"),
                width = 500,
                minWidth = 500,
                autoResize = true,
            };

            MultiColumnHeaderState headerState = new MultiColumnHeaderState(new MultiColumnHeaderState.Column[]{nameColumn, pathColumn});
            MultiColumnHeader header = new MultiColumnHeader(headerState);

            _scenarioInfoTreeView = new ScenarioInfoTreeView(treeViewState, header) {
                searchString = SessionState.GetString(_searchStringSessionStateKey, "")
            };
            _scenarioInfoTreeView.Reload();

            // 検索欄
            _searchField = new SearchField();
        }

        public void DrawLayout(Rect windowRect) {
            var buttonStyle = new GUIStyle("AppToolbarButtonLeft");

            float headerButtonWidth = 80;
            float headerButtonHeight = 24;
            float currentHeaderButtonX = 5;

            if(GUI.Button(new Rect(currentHeaderButtonX, 0, headerButtonWidth, headerButtonHeight), "New", buttonStyle)) {
                ScenarioUtility.CreateScenario();
            }

            if(_scenarioInfoTreeView.HasAny) {
                GUILayout.BeginVertical();
                EditorGUILayout.GetControlRect(GUILayout.Height(6));
                // -------- 先頭行Start --------
                GUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                GUILayout.FlexibleSpace();
                string searchString = _searchField.OnToolbarGUI(_scenarioInfoTreeView.searchString);
                if(EditorGUI.EndChangeCheck()) {
                    _scenarioInfoTreeView.searchString = searchString;
                    SessionState.SetString(_searchStringSessionStateKey, searchString);
                }

                GUILayout.EndHorizontal();
                // -------- 先頭行End --------
                GUILayout.EndVertical();
                Rect treeVeiwRect = new Rect(5, EditorGUIUtility.singleLineHeight + 10, windowRect.width - 10, windowRect.height - 34);

                using(new BackgroundColorScope(new Color(0.8f, 0.8f, 0.8f))) {
                    _scenarioInfoTreeView.OnGUI(treeVeiwRect);
                }
            }
            else {
                EditorGUILayout.HelpBox("Scenario not found.", MessageType.Info);
            }
        }
    }
}