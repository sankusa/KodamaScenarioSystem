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
        private static readonly Color _selectionBorderColor = new Color(0.6f, 1, 0.5f);

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
            if(page.Scenario.DefaultPage == page) {
                Label defaultLabel = new Label("<Default>");
                defaultLabel.style.alignSelf = Align.Center;
                titleContainer.Insert(0, defaultLabel);
                titleContainer.style.backgroundColor = new Color(0.0355f, 0.4433f, 0.2686f, 0.95f);
            }

            VisualElement selectionBorder = Children().FirstOrDefault(x => x.name == _selectionBorderName);
            selectionBorder.style.borderTopColor = _selectionBorderColor;
            selectionBorder.style.borderBottomColor = _selectionBorderColor;
            selectionBorder.style.borderLeftColor = _selectionBorderColor;
            selectionBorder.style.borderRightColor = _selectionBorderColor;
            
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