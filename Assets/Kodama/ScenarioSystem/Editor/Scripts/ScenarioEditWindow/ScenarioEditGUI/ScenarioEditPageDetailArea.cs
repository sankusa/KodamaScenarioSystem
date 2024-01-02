using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditPageDetailArea {
        private ReorderableList _commandList;
        private int _pageInstanceIdOld;
        private bool _commandParameterChanged = false;

        private Stack<string> _indentBlockTypeStack = new Stack<string>();

        private const int _indentWidth = 32;

        // コマンドのパラメータが変更され、サマリの行数に変更があっても
        // 要素の増減や入れ替えがあるまでReorderableListのElementHeightが更新されないので
        // その場合は外部からフラグを立ててもらい、ReorderableList自体を作り直す
        public void OnCommandParameterChanged() {
            _commandParameterChanged = true;
        }

        public void DrawLayout(ScenarioEditGUIStatus guiStatus, Scenario scenario, SerializedObject serializedPage) {
            ScenarioPage page = serializedPage.targetObject as ScenarioPage;
            bool pageChanged = _pageInstanceIdOld != page.GetInstanceID();
            // コマンド追加系操作ののUndo時、ReorderableListに要素数の減少が反映されず、
            // Undo前の要素数を描画しようとしてNullReferenceExceptionが発生。
            // 対策として、要素数の食い違いがあったらReorderableListを再生成する。
            bool invalidReorderableListSize = _commandList != null ? (_commandList.count != page.Commands.Count) : false;
            if(_commandList == null || pageChanged || _commandParameterChanged || invalidReorderableListSize) {
                _commandList = new ReorderableList(serializedPage, serializedPage.FindProperty("_commands"));

                _commandList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Commands");

                _commandList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    // 初期化
                    if(index == 0) {
                        _indentBlockTypeStack.Clear();
                    }

                    if(index >= page.Commands.Count) return;

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

                    // ブロック終了コマンドならインデント-1
                    if(command is IBlockEnd blockEnd) {
                        if(_indentBlockTypeStack.Count > 0 && blockEnd.BlockType == _indentBlockTypeStack.Peek()) {
                            _indentBlockTypeStack.Pop();
                        }
                    }

                    // インデント
                    int indentLevel = _indentBlockTypeStack.Count;
                    Rect indentRect = new Rect(rect.x, rect.y, _indentWidth * indentLevel + 1, rect.height + 1);

                    Rect contentRect = RectUtil.Margin(rect, _indentWidth * indentLevel, bottomMargin: -1);

                    Rect summaryBoxRect = RectUtil.Margin(contentRect, 8, 54, 3, 3);

                    Rect iconRect = RectUtil.Margin(new Rect(summaryBoxRect.x + 3, summaryBoxRect.y, 24, 24), 3, 3, 3, 3);
                    
                    Rect summaryRect = RectUtil.Margin(summaryBoxRect, commandSetting.Icon != null ? 30 : 8, 3, 3, 3);

                    Rect copyButtonRect = new Rect(contentRect.xMax - 51, contentRect.y + 3, 32 - 8, contentRect.height + 1 - 7);
                    Rect removeButtonRect = new Rect(contentRect.xMax - 32 + 4, contentRect.y + 3, 32 - 8, contentRect.height + 1 - 7);

                    Color backgroundColor = commandGroupSetting.GroupColor;
                    backgroundColor.a = 0.5f;

                    if(indentLevel > 0) {
                        using (new BackgroundColorScope(new Color(0.9f, 0.9f, 0.9f))) {
                            GUI.Box(indentRect, "", GUIStyles.LeanGroupBox);
                        }
                    }
                    using (new BackgroundColorScope(backgroundColor)) {
                        GUI.Box(contentRect, "", GUIStyles.LeanGroupBox);
                    }
                    // EditorGUI.LabelField(labelRect, commandSetting.DisplayName);
                    using (new BackgroundColorScope(new Color(0.7f, 0.7f, 0.7f))) {
                        GUI.Box(summaryBoxRect, "", GUIStyles.LeanGroupBox);
                    }
                    using (new ContentColorScope(commandGroupSetting != null ? commandGroupSetting.GroupColor : Color.white)) {
                        EditorGUI.LabelField(iconRect, new GUIContent(commandSetting.Icon, null));
                    }
                    EditorGUI.LabelField(summaryRect, command.GetSummary(), GUIStyles.SummaryLabel);

                    using (new ContentColorScope(new Color(1, 1, 1, 0.5f))) {
                    if(GUI.Button(copyButtonRect, CommonEditorResources.Instance.CommandCopyIcon, GUIStyles.BorderedButton)) {
                        // Undo.RecordObject(page, "Copy Command");
                        // page.InsertCommand(index + 1, page.Commands[index].Copy());
                        // guiStatus.CurrentCommandIndex = index + 1;
                        new CommandDropdown(
                            new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), 
                            commandType => {
                                Undo.RecordObject(page, "Copy Command");
                                page.InsertCommand(index + 1, CommandBase.CreateInstance(commandType, page));
                                guiStatus.CurrentCommandIndex = index + 1;
                            }
                            ).ShowDropDown(new Rect(0, 0, 200, 0));

                    }
                    //GUIContent i = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                    if(GUI.Button(removeButtonRect, CommonEditorResources.Instance.CommandDeleteIcon, GUIStyles.BorderedButton)) {
                        Undo.RecordObject(page, "Delete Command");
                        page.RemoveCommandAt(index);
                    }
                    }

                    if(_commandList.index == index) {
                        using (new BackgroundColorScope(backgroundColor)) {
                            GUI.Box(contentRect, "", GUIStyles.LeanGroupBox);
                        }
                    }

                    // ブロック開始コマンドならインデント+1
                    if(command is IBlockStart blockStart) {
                        _indentBlockTypeStack.Push(blockStart.BlockType);
                    }
                };

                _commandList.elementHeightCallback = index => {
                    int rowCount;
                    if(index < page.Commands.Count) {
                        rowCount = 1 + page.Commands[index].GetSummary().Where(c => c == '\n').Count();
                    }
                    else {
                        rowCount = 1;
                    }
                    return 13 + 15 * rowCount;
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
                            new RectUtil.LayoutLength(0.5f),
                            new RectUtil.LayoutLength(84, RectUtil.LayoutType.Fixed),
                            new RectUtil.LayoutLength(1)
                        }
                    );
                    EditorGUI.LabelField(rects[0], $"Page {guiStatus.CurrentPageIndex + 1}");
                    EditorGUI.LabelField(rects[1], "Page Name : ");

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rects[2], serializedPage.FindProperty("m_Name"), GUIContent.none);
                    if(EditorGUI.EndChangeCheck()) {
                        serializedPage.ApplyModifiedProperties();
                    }
                };
                _commandList.footerHeight = 0;
                _commandList.displayAdd = false;
                _commandList.displayRemove = false;
            }

            _commandList.index = guiStatus.CurrentCommandIndex;
            
            _commandList.DoLayoutList();
            
            _commandParameterChanged = false;
            _pageInstanceIdOld = page.GetInstanceID();
        }
    }
}