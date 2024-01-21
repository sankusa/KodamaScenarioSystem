using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug; 

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class PageDetailArea {
        private int _pageInstanceIdOld;

        private Dictionary<Type, CommandSummaryDrawerBase> _summaryDrawerDic;
        
        private Vector2 _scrollPos;

        private PageCommandsReorderableList _commandList;

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

        public PageDetailArea() {
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
        public void ResizedReorderableList() {
            _commandList?.ResizeReorderableList();
        }

        public void DrawLayout(Rect rectSize, ScenarioEditGUIStatus guiStatus, SerializedObject serializedPage) {
            ScenarioPage page = serializedPage.targetObject as ScenarioPage;
            bool pageChanged = _pageInstanceIdOld != page.GetInstanceID();
            if(pageChanged || _commandList == null) {
                _commandList = new PageCommandsReorderableList(serializedPage, _summaryDrawerDic);
                _scrollPos = Vector2.zero;
            }
            _commandList.Index = guiStatus.CurrentCommandIndex;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _commandList.DrawLayout(rectSize, _scrollPos);
            EditorGUILayout.EndScrollView();
            
            guiStatus.CurrentCommandIndex = _commandList.Index;
            _pageInstanceIdOld = page.GetInstanceID();
        }
    }
}