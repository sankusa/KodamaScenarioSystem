using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class ScenarioEditGUI {
        private ScenarioHeaderArea _scenarioHeader;
        private PageHeaderArea _pageHeader;
        private PageListArea _pageListArea;
        private VariableArea _variableArea;
        private PageDetailArea _pageDetailArea;
        private CommandInspector _commandInspector;
        private CommandGroupArea _commandGroupArea;
        private CommandListArea _commandListArea;
        private ScenarioEditGUIStatus _status;

        private SplitView _pageListSplitView;
        private SplitView _leftAreaSplitView;
        private SplitView _detailAreaSplitView;
        private SplitView _inspectorSplitView;
        private SplitView _commandAreaSplitView;

        private PageGraphView _pageGraphView;

        private ScenarioPage _currentPage;
        private SerializedObject _currentSerializedPage;

        private DateTime _pageLastChangedTime;

        private int _commandInstanceIdOld;
        private int _commandSummaryLineCountOld;

        public ScenarioEditGUI(Scenario scenario, VisualElement rootVisualElement) {
            _scenarioHeader = new ScenarioHeaderArea();
            _pageHeader = new PageHeaderArea();
            _pageListArea = new PageListArea();
            _variableArea = new VariableArea();
            _pageDetailArea = new PageDetailArea();
            _commandInspector = new CommandInspector();
            _commandGroupArea = new CommandGroupArea();
            _commandListArea = new CommandListArea();
            _pageListSplitView = new SplitView(SplitView.Direction.Horizontal, 0.2f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_pageListSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _leftAreaSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_leftAreaSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _detailAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.6f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_detailAreaSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _inspectorSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_inspectorSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _commandAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_commandAreaSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _status = new ScenarioEditGUIStatus();

            _pageGraphView = new PageGraphView(scenario);
            _pageGraphView.OnNodeClick += page => {
                _status.CurrentPageIndex = scenario.Pages.IndexOf(page);
            };
            rootVisualElement.Add(_pageGraphView);
        }

        public void DrawLayout(Rect windowPositiion, Scenario scenario, SerializedObject serializedScenario) {
            // シナリオ更新
            serializedScenario.Update();

            EditorGUI.DrawRect(new Rect(windowPositiion) {x = 0, y = 0}, CommonEditorResources.Instance.BackgroundColor);
            
            using (var change = new EditorGUI.ChangeCheckScope()) {
                _pageListSplitView.Begin();
                DrawLayoutLeftArea(scenario, serializedScenario);

                _pageListSplitView.Split();
                using (new EditorGUILayout.VerticalScope()) {
                    SerializedProperty pagesProp = serializedScenario.FindProperty("_pages");
                    if(
                        // pagesPropの内容がscenarioに反映されていない場合があり、その場合表示しない
                        (pagesProp.arraySize > 0 && scenario.Pages.Count == pagesProp.arraySize)
                        && (0 <= _status.CurrentPageIndex && _status.CurrentPageIndex < pagesProp.arraySize)
                    ) {
                        DrawLayoutPageEditor(scenario, serializedScenario, pagesProp);
                    }
                    else {
                        _scenarioHeader.DrawLayout(scenario, serializedScenario);
                    }
                }

                _pageListSplitView.End();

                if(change.changed) {
                    serializedScenario.ApplyModifiedProperties();
                    _pageDetailArea.ResizedReorderableList();
                }
            }

            if(_pageListSplitView.Resizing || _leftAreaSplitView.Resizing || _commandAreaSplitView.Resizing ||  _inspectorSplitView.Resizing || _detailAreaSplitView.Resizing) {
                EditorWindow.GetWindow<ScenarioEditWindow>().Repaint();
            }
        }

        private void DrawLayoutLeftArea(Scenario scenario, SerializedObject serializedScenario) {
            // ぺージ一覧(画面左)
            _leftAreaSplitView.Begin();
            // _pageListArea.DrawLayout(_status, scenario, serializedObject);
            Rect pageGraphViewRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            // layoutイベント時は更新しない(0でリサイズされたことに起因してrepaintが呼ばれ、結果として毎フレーム画面が更新されてしまう)
            if(pageGraphViewRect.width != 0) {
                _pageGraphView.style.width = pageGraphViewRect.width;
                _pageGraphView.style.height = pageGraphViewRect.height;
            }
            _leftAreaSplitView.Split();
            _variableArea.DrawLayout(_status, scenario, serializedScenario);
            _leftAreaSplitView.End();
        }

        private void DrawLayoutPageEditor(Scenario scenario, SerializedObject serializedScenario, SerializedProperty pagesProp) {
            SerializedProperty pageProp = pagesProp.GetArrayElementAtIndex(_status.CurrentPageIndex);
            ScenarioPage page = pageProp.objectReferenceValue as ScenarioPage;

            if(page != _currentPage) {
                _currentPage = page;
                _currentSerializedPage = new SerializedObject(page);
            }
            
            _currentSerializedPage.Update();
            EditorGUI.BeginChangeCheck();

            SerializedProperty currentPageCommandsProp = _currentSerializedPage.FindProperty("_commands");
            SerializedProperty currentCommandProp = null;
            if(0 <= _status.CurrentCommandIndex && _status.CurrentCommandIndex < currentPageCommandsProp.arraySize) {
                currentCommandProp = currentPageCommandsProp.GetArrayElementAtIndex(_status.CurrentCommandIndex);
            }
            
            using(new EditorGUILayout.VerticalScope()) {
                _detailAreaSplitView.Begin();

                using(new EditorGUILayout.VerticalScope(GUIStyles.LeanGroupBox)) {
                    _scenarioHeader.DrawLayout(scenario, serializedScenario);
                    _pageHeader.DrawLayout(_currentSerializedPage);
                }

                Rect detailAreaRect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                _pageDetailArea.DrawLayout(new Rect(detailAreaRect) {x = 0, y = 0}, _status, _currentSerializedPage);
                EditorGUILayout.EndVertical();

                // コマンドクリップボード操作
                
                if(CommandClipBoard.Any) {
                    EditorGUILayout.LabelField($"{CommandClipBoard.Count} Command Copied");
                }

                _detailAreaSplitView.Split();

                _inspectorSplitView.Begin();
                _commandInspector.DrawLayout(currentCommandProp);

                _inspectorSplitView.Split();
                using(new EditorGUILayout.VerticalScope()) {
                    using(new EditorGUILayout.VerticalScope(GUIStyles.TitleBar)) {
                        EditorGUILayout.LabelField("Command");
                    }
                    
                    _commandAreaSplitView.Begin();
                    _commandGroupArea.DrawLayout(_status);
                    _commandAreaSplitView.Split();
                    _commandListArea.DrawLayout(_status, _currentSerializedPage);
                    _commandAreaSplitView.End();
                }

                _inspectorSplitView.End();
                _detailAreaSplitView.End();
            }

            if(EditorGUI.EndChangeCheck()) {
                _currentSerializedPage.ApplyModifiedProperties();
            }
        }
    }
}