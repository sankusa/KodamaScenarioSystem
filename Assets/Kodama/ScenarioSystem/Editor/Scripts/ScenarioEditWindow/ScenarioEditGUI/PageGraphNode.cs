using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    public class PageGraphNode : Node {
        private readonly ScenarioPage _page;
        public ScenarioPage Page => _page;

        private SerializedObject _serializedPage;

        public event Action<ScenarioPage> OnClick;

        public Vector2 Top => new Vector2(layout.center.x, layout.yMax - 3.5f);
        public Vector2 Bottom => new Vector2(layout.center.x, layout.yMin + 3.5f);

        private const string _selectionBorderName = "selection-border";

        public PageGraphNode(ScenarioPage page) {
            _page = page;
            _serializedPage = new SerializedObject(page);

            titleContainer.Remove(titleButtonContainer);

            titleContainer.style.justifyContent = Justify.Center;
            titleContainer.style.flexDirection = FlexDirection.Column;
            Label label = titleContainer.ElementAt(0) as Label;
            label.style.marginLeft = 16;
            label.style.marginRight = 16;
            label.BindProperty(_serializedPage.FindProperty("m_Name"));
            if(page.ParentScenario.DefaultPage == page) {
                Label defaultLabel = new Label(Labels.Label_DefaultPage);
                defaultLabel.style.alignSelf = Align.Center;
                titleContainer.Insert(0, defaultLabel);
                titleContainer.style.backgroundColor = CommonEditorResources.Instance.EntryNodeColor;
            }
            else {
                titleContainer.style.backgroundColor = CommonEditorResources.Instance.BackgroundColor;
            }

            VisualElement selectionBorder = Children().FirstOrDefault(x => x.name == _selectionBorderName);
            selectionBorder.style.borderTopColor = CommonEditorResources.Instance.NodeSelectionBorder;
            selectionBorder.style.borderBottomColor = CommonEditorResources.Instance.NodeSelectionBorder;
            selectionBorder.style.borderLeftColor = CommonEditorResources.Instance.NodeSelectionBorder;
            selectionBorder.style.borderRightColor = CommonEditorResources.Instance.NodeSelectionBorder;
            
            expanded = false;
            style.left = _page.NodePosition.x;
            style.top = _page.NodePosition.y;

            RegisterCallback<MouseDownEvent>(evt => {
                if(evt.currentTarget == this) {
                    OnClick?.Invoke(_page);
                }
            });
        }

        public void ReflectChangeToPage() {
            _page.NodePosition = new Vector2(style.left.value.value, style.top.value.value);
            EditorUtility.SetDirty(_page);
        }
    }
}