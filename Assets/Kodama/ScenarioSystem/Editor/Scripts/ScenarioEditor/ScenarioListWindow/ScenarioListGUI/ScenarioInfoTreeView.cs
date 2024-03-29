using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    internal class ScenarioInfoTreeView : TreeView {
        public bool HasAny => rootItem.hasChildren;
        
        public ScenarioInfoTreeView(TreeViewState treeViewState, MultiColumnHeader header) : base(treeViewState, header) {
            rowHeight = 20;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }

        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem{depth = -1};

            IEnumerable<string> paths = AssetUtility.GetAllAssetPaths<Scenario>();

            if(paths.Any()) {
                int id = 0;
                foreach(string path in paths) {
                    ScenarioInfoTreeViewItem item = new ScenarioInfoTreeViewItem(++id, path);
                    item.Validate();
                    root.AddChild(item);
                }
            }
            else {
                root.children = new List<TreeViewItem>();
            }

            return root;
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = (ScenarioInfoTreeViewItem)args.item;

            for(int i = 0; i < args.GetNumVisibleColumns(); i++) {
                Rect cellRect = args.GetCellRect(i);
                int columnIndex = args.GetColumn(i);

                if(columnIndex == 0) {
                    EditorGUI.LabelField(cellRect, item.Element.Scenario.name);
                }
                else if(columnIndex == 1) {
                    EditorGUI.LabelField(cellRect, item.Element.Path);
                }
                else if(columnIndex == 2) {
                    if(item.Valid) {
                        EditorGUI.LabelField(cellRect, "<color=green>OK</color>", GUIStyles.SummaryLabel);
                    }
                    else {
                        EditorGUI.LabelField(cellRect, "<color=red>Error</color>", GUIStyles.SummaryLabel);
                    }
                }
            }
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            var scenarioInfoTreeViewItem = (ScenarioInfoTreeViewItem)item;
            string nameLower = scenarioInfoTreeViewItem.Element.Scenario.name.ToLower();
            string searchLower = search.ToLower();

            return nameLower.Contains(searchLower);
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = (ScenarioInfoTreeViewItem)FindItem(id, rootItem);
            ScenarioEditWindow.OpenEditGUI(item.Element.Scenario);
        }

        public void Validate() {
            foreach(TreeViewItem item in rootItem.children) {
                (item as ScenarioInfoTreeViewItem).Validate();
            }
        }
    }
}