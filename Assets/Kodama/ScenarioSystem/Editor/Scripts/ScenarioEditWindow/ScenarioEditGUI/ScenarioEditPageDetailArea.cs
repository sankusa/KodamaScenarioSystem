using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditPageDetailArea {
        private ReorderableList _commandList;
        private Vector2 _scrollPos;
        private string _pathOld;

        internal void DrawLayout(ScenarioEditGUIStatus guiStatus, SerializedProperty pageProp, ScenarioPage page) {
            
            if(_commandList == null || _pathOld != pageProp.propertyPath) {
                _commandList = new ReorderableList(pageProp.serializedObject, pageProp.FindPropertyRelative("_commands"));

                _commandList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Commands");

                _commandList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    //if(_commandList.count != page.Commands.Count) return;
                    CommandSetting commandSetting = CommandSettingTable.AllSettings
                        .FirstOrDefault(x => {
                            if(x.CommandScript != null) {
                                return x.CommandScript.GetClass() == page.Commands[index].GetType();
                            }
                            else {
                                return false;
                            }
                        });
                    if(commandSetting == null) return;
                    CommandGroupSetting commandGroupSetting = CommandGroupSettingTable.AllSettings.FirstOrDefault(x => x.GroupId == commandSetting.GroupId);
                    
                    Rect iconRect = new Rect(rect.x, rect.y, 32, 32);
                    Rect labelRect = new Rect(rect.x + 32, rect.y, rect.width - 32, 16);

                    Color backgroundColor = commandGroupSetting.GroupColor;
                    backgroundColor.a = 0.25f;

                    using (new BackgroundColorScope(backgroundColor)) {
                        GUI.Box(rect, "", new GUIStyle("GroupBox"));
                    }
                    using (new ContentColorScope(commandGroupSetting != null ? commandGroupSetting.GroupColor : Color.white)) {
                        EditorGUI.LabelField(iconRect, new GUIContent(commandSetting.Icon, null));
                    }
                    EditorGUI.LabelField(labelRect, commandSetting.DisplayName);

                    if(_commandList.index == index) {
                        backgroundColor.a = 0.5f;
                        using (new BackgroundColorScope(backgroundColor)) {
                            GUI.Box(rect, "", new GUIStyle("GroupBox"));
                        }
                    }
                };

                _commandList.elementHeightCallback = index => {
                    return 32;
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

            _pathOld = pageProp.propertyPath;
        }
    }
}