using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditGUI {
        private ScenarioEditHeaderArea _headerArea;
        private ScenarioEditPageListArea _pageListArea;
        private ScenarioEditVariableArea _variableArea;
        private ScenarioEditPageHeaderArea _pageHeaderArea;
        private ScenarioEditPageDetailArea _pageDetailArea;
        private ScenarioEditCommandGroupArea _commandGroupArea;
        private ScenarioEditCommandListArea _commandListArea;
        private ScenarioEditGUIStatus _status;

        private SplitView _pageListSplitView;
        private SplitView _leftAreaSplitView;
        private SplitView _detailAreaSplitView;
        private SplitView _inspectorSplitView;
        private SplitView _commandAreaSplitView;

        public ScenarioEditGUI() {
            _headerArea = new ScenarioEditHeaderArea();
            _pageListArea = new ScenarioEditPageListArea();
            _variableArea = new ScenarioEditVariableArea();
            _pageHeaderArea = new ScenarioEditPageHeaderArea();
            _pageDetailArea = new ScenarioEditPageDetailArea();
            _commandGroupArea = new ScenarioEditCommandGroupArea();
            _commandListArea = new ScenarioEditCommandListArea();
            _pageListSplitView = new SplitView(SplitView.Direction.Horizontal, 0.2f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_pageListSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _leftAreaSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_leftAreaSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _detailAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.6f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_detailAreaSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _inspectorSplitView = new SplitView(SplitView.Direction.Vertical, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_inspectorSplitView)}", handleColor: new Color(0.2f, 0.2f, 0.2f));
            _commandAreaSplitView = new SplitView(SplitView.Direction.Horizontal, 0.5f, sessionStateKey: $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_commandAreaSplitView)}", handleColor: new Color(0.15f, 0.15f, 0.15f));
            _status = new ScenarioEditGUIStatus();
        }

        public void DrawLayout(ScenarioEditWindowStatus windowStatus, Scenario scenario, SerializedObject serializedObject) {
            var boxStyle = GUIStyles.LeanGroupBox;
            SerializedProperty pagesProp = serializedObject.FindProperty("_pages");

            //using var _ = new BackgroundColorScope(new Color(0.8f, 0.8f, 0.8f));
            
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // ヘッダ
            _headerArea.DrawLayout(scenario, serializedObject);
            
            _pageListSplitView.Begin();

            // ぺージ一覧(画面左)
            //using(new EditorGUILayout.VerticalScope(boxStyle)) {
                _leftAreaSplitView.Begin();
                _pageListArea.DrawLayout(_status, scenario, serializedObject);
                _leftAreaSplitView.Split();
                _variableArea.DrawLayout(_status, scenario, serializedObject);
                _leftAreaSplitView.End();
            //}

            _pageListSplitView.Split();

            // ページが1つ以上ある&pagesPropの内容がscenarioに反映されるまでラグがあるようなのでそのチェック
            if(pagesProp.arraySize > 0 && scenario.Pages.Count == pagesProp.arraySize) {
                // ページが選択されていたら
                if(0 <= _status.CurrentPageIndex && _status.CurrentPageIndex < pagesProp.arraySize) {
                    SerializedProperty currentPageProp = pagesProp.GetArrayElementAtIndex(_status.CurrentPageIndex);
                    SerializedProperty currentPageCommandsProp = currentPageProp.FindPropertyRelative("_commands");
                    SerializedProperty currentCommandProp = null;
                    if(_status.CurrentCommandIndex < currentPageCommandsProp.arraySize) {
                        currentCommandProp = currentPageCommandsProp.GetArrayElementAtIndex(_status.CurrentCommandIndex);
                        currentCommandProp.isExpanded = true;
                    }
                    
                    
                    using(new EditorGUILayout.VerticalScope()) {
                        // using(new EditorGUILayout.VerticalScope(boxStyle)) {
                        //     _pageHeaderArea.DrawLayout(currentPageProp, _status.CurrentPageIndex, pagesProp.arraySize);
                        // }
                        using(new EditorGUILayout.VerticalScope(boxStyle)) {
                        
                            _detailAreaSplitView.Begin();

                            using(new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true))) {
                                _pageDetailArea.DrawLayout(_status, scenario, currentPageProp, scenario.Pages[_status.CurrentPageIndex]);
                            }

                            _detailAreaSplitView.Split();

                            _inspectorSplitView.Begin();

                            using(new EditorGUILayout.VerticalScope(GUIStyles.TitleBar)) {
                                EditorGUILayout.LabelField("コマンドパラメータ編集");
                            }
                            Rect propertyDrawerRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                            propertyDrawerRect.x -= 10;
                            propertyDrawerRect.xMax += 6;
                            propertyDrawerRect.y -= 16;
                            propertyDrawerRect.yMax += 16;
                            if(currentCommandProp != null && propertyDrawerRect.height > 0) {
                                EditorGUI.PropertyField(propertyDrawerRect, currentCommandProp, GUIContent.none, true);
                            }
                            EditorGUILayout.EndHorizontal();

                            _inspectorSplitView.Split();
                            
                            Color _ = GUI.backgroundColor;
                            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
                            using(new EditorGUILayout.VerticalScope(GUIStyles.LeanGroupBox)) {
                                GUI.backgroundColor = _;
                                using(new EditorGUILayout.VerticalScope(GUIStyles.TitleBar)) {
                                    EditorGUILayout.LabelField("コマンド");
                                }
                                
                                _commandAreaSplitView.Begin();
                                _commandGroupArea.DrawLayout(_status);
                                _commandAreaSplitView.Split();
                                _commandListArea.DrawLayout(_status, scenario, scenario.Pages[_status.CurrentPageIndex]);
                                _commandAreaSplitView.End();
                            }

                            _inspectorSplitView.End();
                            _detailAreaSplitView.End();
                        }
                    }
                }
            }
            
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