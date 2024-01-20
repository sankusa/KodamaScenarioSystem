using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        private bool _needRebuildReorderableList = false;

        private Stack<string> _indentBlockTypeStack = new Stack<string>();

        private const int _indentWidth = 16;

        private const float _elementHeightMin = 22;
        private bool _skipDrawElement = true;
        private int _drawElementEndIndex;

        private Vector2 _scrollPos;
        private Rect _rectSize;

        private Dictionary<Type, (CommandGroupSetting, CommandSetting)> _settingDic;

        private Dictionary<Type, CommandSummaryDrawerBase> _summaryDrawerDic;

        private class SummaryDrawerCacheElement {
            public readonly Type[] _types;
            public readonly CommandSummaryDrawerBase _summaryDrawer;
            public SummaryDrawerCacheElement(Type summaryDrawerType) {
                _types = summaryDrawerType
                    .GetCustomAttributes(typeof(CommandSummaryDrawerAttribute), false)
                    .Select(x => (x as CommandSummaryDrawerAttribute).Type).ToArray();

                _summaryDrawer = Activator.CreateInstance(summaryDrawerType) as CommandSummaryDrawerBase;
            }
        }
        private class SummaryDrawerCache {
            private readonly SummaryDrawerCacheElement[] _elements;
            public SummaryDrawerCache() {
                _elements = TypeCache
                    .GetTypesDerivedFrom<CommandSummaryDrawerBase>()
                    .Select(x => new SummaryDrawerCacheElement(x))
                    .ToArray();
            }
            public CommandSummaryDrawerBase GetSummaryDrawer(Type commandType) {
                CommandSummaryDrawerBase target = null;
                while(true) {
                    target = _elements
                        .FirstOrDefault(x => x._types.Contains(commandType))
                        ?._summaryDrawer;

                    if(target != null) return target;

                    commandType = commandType.BaseType;
                    if(commandType is null) return null;
                }
            }
        }

        public ScenarioEditPageDetailArea() {
            SummaryDrawerCache summaryDrawerCache = new SummaryDrawerCache();
            _summaryDrawerDic = new Dictionary<Type, CommandSummaryDrawerBase>();
            foreach(Type commandtype in TypeCache.GetTypesDerivedFrom<CommandBase>().Where(x => x.IsAbstract == false)) {
                _summaryDrawerDic[commandtype] = summaryDrawerCache.GetSummaryDrawer(commandtype);
            }
        }

        // コマンドのパラメータが変更され、サマリの行数に変更があっても
        // ReorderableListのElementHeightが更新されないので
        // サマリ行数の変動を外部で補足してフラグを立ててもらい
        // ReorderableListのキャッシュを消去して再構築する
        public void RebuildReorderableList() {
            _needRebuildReorderableList = true;
        }

        public void DrawLayout(Rect rectSize, ScenarioEditGUIStatus guiStatus, Scenario scenario, SerializedObject serializedPage) {
            _rectSize = rectSize;

            ScenarioPage page = serializedPage.targetObject as ScenarioPage;
            bool pageChanged = _pageInstanceIdOld != page.GetInstanceID();
            // コマンド追加系操作ののUndo時、ReorderableListに要素数の減少が反映されず、
            // Undo前の要素数を描画しようとしてNullReferenceExceptionが発生。
            // 対策として、要素数の食い違いがあったらReorderableListを再生成する。
            bool invalidReorderableListSize = _commandList != null ? (_commandList.count != page.Commands.Count) : false;
            if(_commandList == null || pageChanged || invalidReorderableListSize) {
                _commandList = new ReorderableList(serializedPage, serializedPage.FindProperty("_commands"), true, false, false, false);
                _commandList.showDefaultBackground = false;

                _commandList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Commands");

                _commandList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    rect = new Rect(rect) {xMax = rect.xMax + 6};
                    // 初期化
                    if(index == 0) {
                        _indentBlockTypeStack.Clear();
                        _settingDic = CommandGroupSetting.GenerateCurrentSettingDictonary();
                        _skipDrawElement = true;
                        _drawElementEndIndex = 0;
                    }

                    if(index >= page.Commands.Count) return;

                    CommandBase command = page.Commands[index];
                    //if(_commandList.count != page.Commands.Count) return;
                    // CommandSetting commandSetting = CommandSettingTable.AllSettings
                    //     .FirstOrDefault(x => {
                    //         if(x.CommandScript != null) {
                    //             return x.CommandScript.GetClass() == command.GetType();
                    //         }
                    //         else {
                    //             return false;
                    //         }
                    //     });
                    // SCommandGroupSetting commandGroupSetting = SCommandGroupSetting.All.FirstOrDefault(x => x.GroupId == commandSetting?.GroupId);

                    // ブロック終了コマンドならインデント-1
                    if(command is IBlockEnd blockEnd) {
                        if(_indentBlockTypeStack.Count > 0 && blockEnd.BlockType == _indentBlockTypeStack.Peek()) {
                            _indentBlockTypeStack.Pop();
                        }
                    }

                    // ScrollViewの描画範囲外なら描画処理を行わない
                    // → CommandInspector編集中にサマリの行数が変わって描画する要素数が変わる
                    // → フィールドのフォーカスが勝手に外れる現象が発生(描画要素数が変わってフォーカスがずれた？)
                    // → サマリの高さは常に最少ということにして描画の可否を決定することで対応
                    if(_skipDrawElement) {
                        Rect elementRealRect = rect; // スクロール分を補正した実際の描画範囲
                        elementRealRect.x -= _scrollPos.x;
                        elementRealRect.y -= _scrollPos.y;
                        if(_rectSize.Overlaps(elementRealRect)) {
                            _skipDrawElement = false;
                            _drawElementEndIndex = index + (int)(_rectSize.height / _elementHeightMin) - 1;
                        }
                    }

                    if(_skipDrawElement == false && index <= _drawElementEndIndex) {
                        // インデント
                        int indentLevel = _indentBlockTypeStack.Count;
                        Rect indentRect = new Rect(rect.x, rect.y, _indentWidth * indentLevel + 1, rect.height + 1);

                        Rect contentRect = RectUtil.Margin(rect, _indentWidth * indentLevel, bottomMargin: -1);

                        Rect summaryBoxRect = RectUtil.Margin(contentRect, 0, 68, 0, 1);
                        Rect summaryBoxInnerRect = RectUtil.Margin(summaryBoxRect, 1, 1, 1, 1);

                        Rect menuButtonRect = new Rect(contentRect.xMax - 66, contentRect.y + 1, 22, contentRect.height - 3);
                        Rect copyButtonRect = new Rect(contentRect.xMax - 45, contentRect.y + 1, 22, contentRect.height - 3);
                        Rect removeButtonRect = new Rect(contentRect.xMax - 24, contentRect.y + 1, 22, contentRect.height - 3);

                        if(indentLevel > 0) {
                            using (new BackgroundColorScope(new Color(0.5f, 0.5f, 0.5f))) {
                                GUI.Box(indentRect, "", GUIStyles.LeanGroupBox);
                            }
                        }
                        GUI.Box(summaryBoxRect, "", GUIStyles.LeanGroupBox);

                        // サマリ表示
                        (CommandGroupSetting commandGroupSetting, CommandSetting commandSetting) settings;
                        _settingDic.TryGetValue(command.GetType(), out settings);

                        if(settings != default) {
                            Color groupColor = settings.commandGroupSetting != null ? settings.commandGroupSetting.Color : Color.white;
                            EditorGUI.DrawRect(summaryBoxInnerRect, groupColor);

                            CommandSummaryDrawerBase summaryDrawer = _summaryDrawerDic[command.GetType()];
                            summaryDrawer.Draw(summaryBoxInnerRect, command, settings.commandGroupSetting, settings.commandSetting);
                        }
                        else {
                            EditorGUI.LabelField(summaryBoxInnerRect, command.GetType().Name);
                        }

                        using (new ContentColorScope(new Color(1, 1, 1, 0.5f))) {
                        if(GUI.Button(menuButtonRect, CommonEditorResources.Instance.MenuIcon, GUIStyles.BorderedButton)) {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(
                                new GUIContent("Copy"),
                                on: false,
                                func: () => CommandClipBoard.CopyToClipBoard(new List<CommandBase>(){command})
                            );
                            menu.DropDown(menuButtonRect);
                        }
                        if(GUI.Button(copyButtonRect, CommonEditorResources.Instance.CommandCopyIcon, GUIStyles.BorderedButton)) {
                            Undo.RecordObject(page, "Duplicate Command");
                            page.InsertCommand(index + 1, page.Commands[index].Copy());
                            guiStatus.CurrentCommandIndex = index + 1;
                            // new CommandDropdown(
                            //     new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), 
                            //     commandType => {
                            //         Undo.RecordObject(page, "Copy Command");
                            //         page.InsertCommand(index + 1, CommandBase.CreateInstance(commandType, page));
                            //         guiStatus.CurrentCommandIndex = index + 1;
                            //     }
                            //     ).ShowDropDown(new Rect(0, 0, 200, 0));

                        }
                        //GUIContent i = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                        if(GUI.Button(removeButtonRect, CommonEditorResources.Instance.CommandDeleteIcon, GUIStyles.BorderedButton)) {
                            Undo.RecordObject(page, "Delete Command");
                            page.RemoveCommandAt(index);
                        }
                        }

                        if(_commandList.index == index) {
                            EditorGUI.DrawRect(contentRect, new Color(1, 1, 1, 0.1f));
                        }
                    }

                    // ブロック開始コマンドならインデント+1
                    if(command is IBlockStart blockStart) {
                        _indentBlockTypeStack.Push(blockStart.BlockType);
                    }
                };

                _commandList.elementHeightCallback = index => {
                    // 初期化
                    if(index == 0) {
                        _settingDic = CommandGroupSetting.GenerateCurrentSettingDictonary();
                    }

                    CommandBase command = page.Commands[index];

                    (CommandGroupSetting commandGroupSetting, CommandSetting commandSetting) settings;
                    _settingDic.TryGetValue(command.GetType(), out settings);

                    if(settings == default) return 21;

                    CommandSummaryDrawerBase summaryDrawer = _summaryDrawerDic[command.GetType()];
                    return summaryDrawer.GetHeight(command, settings.commandGroupSetting, settings.commandSetting);
                };

                _commandList.onSelectCallback = list => {
                    guiStatus.CurrentCommandIndex = list.index;
                };

                _commandList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => {};

                _commandList.footerHeight = 0;
                _commandList.displayAdd = false;
                _commandList.displayRemove = false;
            }

            if(_needRebuildReorderableList) {
                typeof(ReorderableList).GetField("lastRect", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_commandList, Rect.zero);
            }

            _commandList.index = guiStatus.CurrentCommandIndex;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                _commandList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            
            _needRebuildReorderableList = false;
            _pageInstanceIdOld = page.GetInstanceID();
        }
    }
}