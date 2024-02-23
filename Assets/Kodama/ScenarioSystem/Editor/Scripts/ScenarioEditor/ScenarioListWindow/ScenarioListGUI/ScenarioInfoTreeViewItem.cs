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

        public bool Valid {get; private set;}

        public ScenarioInfoTreeViewItem(int id, string path) : base(id) {
            Element = new ScenarioInfo(path);
        }

        public void Validate() {
            Valid = string.IsNullOrEmpty(Element.Scenario.Validate());
        }
    }
}