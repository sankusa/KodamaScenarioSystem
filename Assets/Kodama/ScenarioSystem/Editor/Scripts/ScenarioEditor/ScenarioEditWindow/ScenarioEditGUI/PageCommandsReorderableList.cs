using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class PageCommandsReorderableList {
        private ReorderableList _commandList;

        private Stack<string> _indentBlockTypeStack = new Stack<string>();
        private const int _indentWidth = 16;

        private const float _elementHeightMin = 22;
        private bool _skipDrawElement = true;
        private int _drawElementEndIndex;

        private Rect _rectSize;
        private Vector2 _scrollPos;

        private Dictionary<Type, (CommandGroupSetting, CommandSetting)> _settingDic;

        private bool _isFocused;

        private int _currentIndex = -1;
        public int Index {
            get => _currentIndex;
            set => _currentIndex = value;
        }

        private int _indexOld = -1;

        private SerializedObject _serializedPage;
        private ScenarioPage _page;
        private List<CommandBase> _selections = new List<CommandBase>();
        private Dictionary<Type, CommandSummaryDrawerBase> _summaryDrawerDic;

        private int _backgroundControlId;

        public PageCommandsReorderableList(SerializedObject serializedPage, Dictionary<Type, CommandSummaryDrawerBase> summaryDrawerDic) {
            _serializedPage = serializedPage;
            _page = serializedPage.targetObject as ScenarioPage;
            _summaryDrawerDic = summaryDrawerDic;
            RegenerateReorderableList();
            _backgroundControlId = EditorGUIUtility.GetControlID(FocusType.Passive);
        }

        public void DrawLayout(Rect rect, Vector2 scrollPos) {
            _rectSize = new Rect(rect) {x = 0, y = 0};
            _scrollPos = scrollPos;

            // コマンド追加系操作ののUndo時、ReorderableListに要素数の減少が反映されず、
            // Undo前の要素数を描画しようとしてNullReferenceExceptionが発生。
            // 対策として、要素数の食い違いがあったらReorderableListを再生成する。
            bool invalidReorderableListSize = _commandList != null ? (_commandList.count != _page.Commands.Count) : false;
            if(invalidReorderableListSize) {
                RegenerateReorderableList();
            }

            // セレクション更新
            // Destroy済みならRemove
            for(int i = _selections.Count - 1; i >= 0; i--) {
                if(_selections[i] == null) _selections.RemoveAt(i);
            }
            // インデックス変更されていたらがインデックスの指す先を選択
            if(_currentIndex != _indexOld) {
                if(0 <= _currentIndex && _currentIndex < _page.Commands.Count) {
                    _selections.Clear();
                    _selections.Add(_page.Commands[_currentIndex]);
                }
                else {
                    _selections.Clear();
                }
            }

            _commandList.DoLayoutList();

            if(Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
                EditorGUIUtility.hotControl = _backgroundControlId;
                EditorGUIUtility.keyboardControl = _backgroundControlId;
            }

            // 要素がフォーカスされていなくても背景がフォーカスされていればフォーカスされている扱いとする
            if(_isFocused == false && EditorGUIUtility.hotControl == _backgroundControlId) _isFocused = true;

            // コピー&ペースト関連処理
            if(_isFocused && Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.C) {
                CopySelections();
            }
            else if(_isFocused && Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.X) {
                CutSelections();
            }
            else if(_isFocused && Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.V) {
                PasteFromCommandClipboard();
            }

            _indexOld = _currentIndex;
        }

        public void ResizeReorderableList() {
            typeof(ReorderableList).GetField("lastRect", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_commandList, Rect.zero);
        }

        public void RegenerateReorderableList() {
            _commandList = new ReorderableList(_serializedPage, _serializedPage.FindProperty("_commands"), true, false, false, false);
            _commandList.showDefaultBackground = false;

            _commandList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Commands");

            _commandList.drawElementCallback = (rect, index, isActive, isFocused) => {
                _commandList.draggable = true;
                rect = new Rect(rect) {xMax = rect.xMax + 6};
                // 初期化
                if(index == 0) {
                    _indentBlockTypeStack.Clear();
                    _settingDic = CommandGroupSetting.GenerateCurrentSettingDictonary();
                    _skipDrawElement = true;
                    _drawElementEndIndex = 0;
                    _isFocused = false;
                }

                _isFocused |= isFocused;

                if(index >= _page.Commands.Count) return;

                CommandBase command = _page.Commands[index];

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

                    Rect summaryBoxRect = RectUtil.Margin(contentRect, 0, 46, 0, 1);
                    Rect summaryBoxInnerRect = RectUtil.Margin(summaryBoxRect, 1, 1, 1, 1);

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
                        if(GUI.Button(copyButtonRect, CommonEditorResources.Instance.CommandCopyIcon, GUIStyles.BorderedButton)) {
                            CommandBase copied = command.CopyWithUndo();
                            _page.InsertCommand(index + 1, copied);
                            _selections.Clear();
                            _selections.Add(copied);
                            _currentIndex = index + 1;
                            // new CommandDropdown(
                            //     new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), 
                            //     commandType => {
                            //         Undo.RecordObject(page, "Copy Command");
                            //         page.InsertCommand(index + 1, CommandBase.CreateInstance(commandType, page));
                            //         guiStatus.CurrentCommandIndex = index + 1;
                            //     }
                            //     ).ShowDropDown(new Rect(0, 0, 200, 0));
                        }
                        if(GUI.Button(removeButtonRect, CommonEditorResources.Instance.CommandDeleteIcon, GUIStyles.BorderedButton)) {
                            _page.RemoveAndDestroyCommand(command);
                            _selections.Remove(command);
                            if(index < _currentIndex) {
                                _currentIndex--;
                            }
                            else if(index == _currentIndex && index == _page.Commands.Count) {
                                _currentIndex--;
                            }
                        }
                    }

                    if(_selections.Contains(command)) {
                        EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
                    }

                    if(index == _currentIndex) {
                        EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
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

                if(index >= _page.Commands.Count) return _elementHeightMin;

                CommandBase command = _page.Commands[index];

                (CommandGroupSetting commandGroupSetting, CommandSetting commandSetting) settings;
                _settingDic.TryGetValue(command.GetType(), out settings);

                if(settings == default) return _elementHeightMin;

                CommandSummaryDrawerBase summaryDrawer = _summaryDrawerDic[command.GetType()];
                return summaryDrawer.GetHeight(command, settings.commandGroupSetting, settings.commandSetting);
            };

            _commandList.onSelectCallback = list => {
                if(Event.current.control) {
                    CommandBase target = _page.Commands[list.index];
                    if(_selections.Contains(target) == false) {
                        _selections.Add(target);
                    }
                }
                else if(Event.current.shift) {
                    int fromIndex = Mathf.Min(_indexOld, list.index);
                    int toIndex = Mathf.Max(_indexOld, list.index);
                    for(int i = fromIndex; i <= toIndex; i++) {
                        if(i < 0 || _page.Commands.Count < i) continue;
                        CommandBase target = _page.Commands[i];
                        if(_selections.Contains(target) == false) {
                            _selections.Add(target);
                        }
                    }
                }
                else {
                    _selections.Clear();
                    _selections.Add(_page.Commands[list.index]);
                }
                _currentIndex = list.index;
            };

            _commandList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => {
                // ドラッグ機能を維持したままドラッグハンドルの描画(drawElementBackgroundとdrawElementの間に描画される)をスキップするため
                // 一時的にdraggableをfalseにしてdrawElementでtrueに戻す
                _commandList.draggable = false;
            };

            _commandList.footerHeight = 0;
            _commandList.displayAdd = false;
            _commandList.displayRemove = false;
        }

        private void CopySelections() {
            if(_selections.Count == 0) return;
            CommandClipBoard.CopyToClipBoard(_selections.OrderBy(x => x.Index));
        }

        private void CutSelections() {
            if(_selections.Count == 0) return;
            IEnumerable<CommandBase> sorted = _selections.OrderBy(x => x.Index);
            int firstIndex = sorted.First().Index;
            CommandClipBoard.CopyToClipBoard(sorted);
            _selections.ForEach(x => x.ParentPage.RemoveAndDestroyCommand(x));
            _selections.Clear();
            if(firstIndex < _page.Commands.Count) {
                _selections.Add(_page.Commands[firstIndex]);
                _currentIndex = firstIndex;
            }
            else if(_page.Commands.Count != 0) {
                _selections.Add(_page.Commands.Last());
                _currentIndex = _page.Commands.Count;
            }
            else {
                _currentIndex = -1;
            }
        }

        private void PasteFromCommandClipboard() {
            List<CommandBase> copiedCommands = CommandClipBoard.CopyFromClipBoardWithUndo(_page);
            int insertIndex = Mathf.Min(_currentIndex + 1, _page.Commands.Count - 1);
            _page.InsertCommands(insertIndex, copiedCommands);
           _selections.Clear();
           _selections.AddRange(copiedCommands);
           _currentIndex = insertIndex + copiedCommands.Count - 1;
        }
    }
}