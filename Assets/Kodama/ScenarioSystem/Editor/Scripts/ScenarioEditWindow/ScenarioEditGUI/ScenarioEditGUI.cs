using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditGUI {
        private ScenarioEditHeaderArea _headerArea;
        private ScenarioEditPageListArea _pageListArea;
        private ScenarioEditVariableArea _variableArea;
        private ScenarioEditPageDetailArea _pageDetailArea;
        private ScenarioEditCommandInspector _commandInspector;
        private ScenarioEditCommandGroupArea _commandGroupArea;
        private ScenarioEditCommandListArea _commandListArea;
        private ScenarioEditGUIStatus _status;

        private SplitView _pageListSplitView;
        private SplitView _leftAreaSplitView;
        private SplitView _detailAreaSplitView;
        private SplitView _inspectorSplitView;
        private SplitView _commandAreaSplitView;

        private PageGraphView _pageGraphView;

        public ScenarioEditGUI(Scenario scenario, VisualElement rootVisualElement) {
            _headerArea = new ScenarioEditHeaderArea();
            _pageListArea = new ScenarioEditPageListArea();
            _variableArea = new ScenarioEditVariableArea();
            _pageDetailArea = new ScenarioEditPageDetailArea();
            _commandInspector = new ScenarioEditCommandInspector();
            _commandGroupArea = new ScenarioEditCommandGroupArea();
            _commandListArea = new ScenarioEditCommandListArea();
            _pageListSplitView = new SplitView(SplitView.Direction.Horizontal, 0.2f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_pageListSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _leftAreaSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_leftAreaSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _detailAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.6f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_detailAreaSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _inspectorSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_inspectorSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _commandAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_commandAreaSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _status = new ScenarioEditGUIStatus();

            _pageGraphView = new PageGraphView(scenario);
            _pageGraphView.OnNodeClick += page => {
                _status.CurrentPageIndex = scenario.Pages.IndexOf(page);
            };
            rootVisualElement.Add(_pageGraphView);
        }

        public void DrawLayout(ScenarioEditWindowStatus windowStatus, Scenario scenario, SerializedObject serializedObject) {
            var boxStyle = GUIStyles.LeanGroupBox;
            SerializedProperty pagesProp = serializedObject.FindProperty("_pages");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            _pageListSplitView.Begin();

            // ぺージ一覧(画面左)
            // using(new EditorGUILayout.VerticalScope(boxStyle)) {
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
                _variableArea.DrawLayout(_status, scenario, serializedObject);
                _leftAreaSplitView.End();
            // }

            _pageListSplitView.Split();

            EditorGUILayout.BeginVertical();

            // ヘッダ
            _headerArea.DrawLayout(scenario, serializedObject);

            // ページが1つ以上ある&pagesPropの内容がscenarioに反映されるまでラグがあるようなのでそのチェック
            if(pagesProp.arraySize > 0 && scenario.Pages.Count == pagesProp.arraySize) {
                // ページが選択されていたら
                if(0 <= _status.CurrentPageIndex && _status.CurrentPageIndex < pagesProp.arraySize) {
                    SerializedProperty currentPageProp = pagesProp.GetArrayElementAtIndex(_status.CurrentPageIndex);

                    SerializedObject serializedPage = new SerializedObject(currentPageProp.objectReferenceValue);
                    serializedPage.Update();
                    EditorGUI.BeginChangeCheck();

                    SerializedProperty currentPageCommandsProp = serializedPage.FindProperty("_commands");
                    SerializedProperty currentCommandProp = null;
                    if(_status.CurrentCommandIndex < currentPageCommandsProp.arraySize) {
                        currentCommandProp = currentPageCommandsProp.GetArrayElementAtIndex(_status.CurrentCommandIndex);
                        currentCommandProp.isExpanded = true;
                    }
                    
                    
                    using(new EditorGUILayout.VerticalScope()) {
                        using(new EditorGUILayout.VerticalScope(boxStyle)) {
                        
                            _detailAreaSplitView.Begin();

                            using(new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true))) {
                                _pageDetailArea.DrawLayout(_status, scenario, serializedPage);
                            }

                            _detailAreaSplitView.Split();

                            _inspectorSplitView.Begin();

                            _commandInspector.DrawLayout(currentCommandProp);

                            _inspectorSplitView.Split();
                            
                            Color _ = GUI.backgroundColor;
                            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
                            using(new EditorGUILayout.VerticalScope(GUIStyles.LeanGroupBox)) {
                                GUI.backgroundColor = _;
                                using(new EditorGUILayout.VerticalScope(GUIStyles.TitleBar)) {
                                    EditorGUILayout.LabelField("Command Select");
                                }
                                
                                _commandAreaSplitView.Begin();
                                _commandGroupArea.DrawLayout(_status);
                                _commandAreaSplitView.Split();
                                _commandListArea.DrawLayout(_status, serializedPage);
                                _commandAreaSplitView.End();
                            }

                            _inspectorSplitView.End();
                            _detailAreaSplitView.End();
                        }
                    }

                    if(EditorGUI.EndChangeCheck()) {
                        serializedPage.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUILayout.EndVertical();
            
            _pageListSplitView.End();

            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
                _pageDetailArea.OnCommandParameterChanged();
            }

            if(_pageListSplitView.Resizing || _leftAreaSplitView.Resizing || _commandAreaSplitView.Resizing ||  _inspectorSplitView.Resizing || _detailAreaSplitView.Resizing) {
                EditorWindow.GetWindow<ScenarioEditWindow>().Repaint();
            }
        }
    }
}