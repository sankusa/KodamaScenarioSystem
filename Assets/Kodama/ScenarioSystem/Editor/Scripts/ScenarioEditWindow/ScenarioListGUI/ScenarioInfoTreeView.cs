using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioInfoTreeView : TreeView {
        internal bool HasAny => rootItem.hasChildren;
        
        internal ScenarioInfoTreeView(TreeViewState treeViewState, MultiColumnHeader header) : base(treeViewState, header) {
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
                    root.AddChild(new ScenarioInfoTreeViewItem(++id, path));
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
    }
}