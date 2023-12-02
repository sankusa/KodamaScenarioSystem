using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditPageDetailArea {
        private ReorderableList _commandList;
        private Vector2 _scrollPos;
        private string _pathOld;

        private bool _commandParameterChanged = false;

        // コマンドのパラメータが変更され、サマリの行数に変更があっても
        // 要素の増減や入れ替えがあるまでReorderableListのElementHeightが更新されないので
        // その場合は外部からフラグを立ててもらい、ReorderableList自体を作り直す
        public void OnCommandParameterChanged() {
            _commandParameterChanged = true;
        }

        public void DrawLayout(ScenarioEditGUIStatus guiStatus, SerializedProperty pageProp, ScenarioPage page) {
            if(_commandList == null || _pathOld != pageProp.propertyPath || _commandParameterChanged) {
                _commandList = new ReorderableList(pageProp.serializedObject, pageProp.FindPropertyRelative("_commands"));

                _commandList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Commands");

                _commandList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    CommandBase command = page.Commands[index];
                    //if(_commandList.count != page.Commands.Count) return;
                    CommandSetting commandSetting = CommandSettingTable.AllSettings
                        .FirstOrDefault(x => {
                            if(x.CommandScript != null) {
                                return x.CommandScript.GetClass() == command.GetType();
                            }
                            else {
                                return false;
                            }
                        });
                    if(commandSetting == null) return;
                    CommandGroupSetting commandGroupSetting = CommandGroupSettingTable.AllSettings.FirstOrDefault(x => x.GroupId == commandSetting.GroupId);
                    
                    Rect iconRect = new Rect(rect.x, rect.y, 28, 28);
                    Rect labelRect = new Rect(rect.x + 32, rect.y, rect.width - 32, 16);
                    Rect summaryBoxRect = new Rect(rect.x + 32, rect.y + 3, rect.width - 32, rect.height - 6);
                    Rect summaryRect = new Rect(rect.x + 32 + 4, rect.y + 3, rect.width - 32, rect.height - 6);

                    Color backgroundColor = commandGroupSetting.GroupColor;
                    backgroundColor.a = 0.5f;

                    using (new BackgroundColorScope(backgroundColor)) {
                        GUI.Box(new Rect(rect){height = rect.height + 1}, "", GUIStyles.LeanGroupBox);
                    }
                    using (new ContentColorScope(commandGroupSetting != null ? commandGroupSetting.GroupColor : Color.white)) {
                        EditorGUI.LabelField(iconRect, new GUIContent(commandSetting.Icon, null));
                    }
                    // EditorGUI.LabelField(labelRect, commandSetting.DisplayName);
                    using (new BackgroundColorScope(new Color(0.7f, 0.7f, 0.7f))) {
                        GUI.Box(summaryBoxRect, "", GUIStyles.LeanGroupBox);
                    }
                    var x = new GUIStyle(EditorStyles.label);
                    x.richText = true;
                    EditorGUI.LabelField(summaryRect, command.GetSummary(), GUIStyles.SummaryLabel);

                    if(_commandList.index == index) {
                        backgroundColor.a = 0.5f;
                        using (new BackgroundColorScope(backgroundColor)) {
                            GUI.Box(rect, "", GUIStyles.LeanGroupBox);
                        }
                    }
                };

                _commandList.elementHeightCallback = index => {
                    return 13 + 15 * (1 + page.Commands[index].GetSummary().Where(c => c == '\n').Count());
                };

                _commandList.onSelectCallback = list => {
                    guiStatus.CurrentCommandIndex = list.index;
                };

                _commandList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => {};

                _commandList.drawHeaderCallback = rect => {
                    Rect headerRect = new Rect(rect.x - 4, rect.y, rect.width + 9, rect.height);
                    GUI.Box(headerRect, "", GUIStyles.TitleBar);
                    //EditorGUI.LabelField(headerRect, "ページ内容");

                    List<Rect> rects = RectUtil.DivideRectHorizontal(
                        headerRect,
                        new RectUtil.LayoutLength[]{
                            new RectUtil.LayoutLength(1),
                            new RectUtil.LayoutLength(60, RectUtil.LayoutType.Fixed),
                            new RectUtil.LayoutLength(1)
                        }
                    );
                    EditorGUI.LabelField(rects[0], $"ページ {guiStatus.CurrentPageIndex + 1}");
                    EditorGUI.LabelField(rects[1], "ページ名");
                    EditorGUI.PropertyField(rects[2], pageProp.FindPropertyRelative("_name"), GUIContent.none);
                };
                _commandList.footerHeight = 0;
                _commandList.displayAdd = false;
                _commandList.displayRemove = false;
            }

            _commandList.index = guiStatus.CurrentCommandIndex;
            
            _commandList.DoLayoutList();
            
            _commandParameterChanged = false;
            _pathOld = pageProp.propertyPath;
        }
    }
}