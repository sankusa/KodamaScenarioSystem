using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// シナリオ情報TreeView要素
    /// </summary>
    internal class ScenarioInfoTreeViewItem : TreeViewItem {
        public ScenarioInfo Element {get;}

        public ScenarioInfoTreeViewItem(int id, string path) : base(id) {
            Element = new ScenarioInfo(path);
        }
    }
}