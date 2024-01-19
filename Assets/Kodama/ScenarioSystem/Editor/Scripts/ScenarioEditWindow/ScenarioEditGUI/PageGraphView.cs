using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace Kodama.ScenarioSystem.Editor {
    internal class PageGraphView : GraphView {
        private readonly Scenario _scenario;
        private readonly List<PageGraphNode> _nodes = new List<PageGraphNode>();

        public event Action<ScenarioPage> OnNodeClick;

        private SelectionDragger _selectionDragger;

        public PageGraphView(Scenario scenario) : base() {
            _scenario = scenario;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            styleSheets.Add(Resources.Load<StyleSheet>("GridBackGround"));
            GridBackground gridBackground = new GridBackground() {};
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();

            contentViewContainer.transform.position = scenario.GraphPosition;

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));                 
            this.AddManipulator(new SelectionDragger() {panSpeed = Vector2.zero});

            graphViewChanged += change => {
                _nodes.ForEach(x => x.ReflectChangeToPage());
                return change;
            };
            
            viewTransformChanged += change => {
                _scenario.GraphPosition = viewTransform.position;
                EditorUtility.SetDirty(_scenario);
            } ;

            Rebuild();

            Undo.undoRedoPerformed += Rebuild;

            DrawImmediateElement drawImmediateElement = new DrawImmediateElement();
            drawImmediateElement.OnRepaint += DrawEdge;
            Add(drawImmediateElement);
        }

        public void Rebuild() {
            // インスタンスが破棄されたことをpanelで判定し、コールバックを解除する
            if(panel == null) {
                Undo.undoRedoPerformed -= Rebuild;
            }

            _nodes.Clear();
            if(contentViewContainer.childCount > 0) contentViewContainer.ElementAt(0).Clear();
            // ノード作成
            foreach(ScenarioPage page in _scenario.Pages){
                CreateNode(page);
            }
        }

        private void CreateNode(ScenarioPage page) {
            PageGraphNode node = new PageGraphNode(page);
            AddElement(node);
            _nodes.Add(node);
            node.OnClick += page => {
                OnNodeClick?.Invoke(page);
            };
        }

        private void OnContextMenuPopulate(ContextualMenuPopulateEvent e) {
            if((e.target is PageGraphNode) == false) {
                e.menu.InsertAction (
                    0,
                    "Create Page",
                    CreatePage,
                    DropdownMenuAction.AlwaysEnabled
                );
            }
            if(e.target is PageGraphNode) {
                e.menu.InsertAction (
                    0,
                    "Set Default",
                    SetDefaultPage,
                    DropdownMenuAction.AlwaysEnabled
                );
            }

        }

        private void CreatePage(DropdownMenuAction menuAction) {
            ScenarioPage newPage = _scenario.CreatePage();
            newPage.NodePosition = (menuAction.eventInfo.localMousePosition - (Vector2)contentViewContainer.transform.position) / contentViewContainer.transform.scale.x;

            CreateNode(newPage);
        }

        private void SetDefaultPage(DropdownMenuAction menuAction) {
            PageGraphNode node = selection[0] as PageGraphNode;
            node.Page.Scenario.ChangeDefaultPage(node.Page);

            Rebuild();
        }

        public override EventPropagation DeleteSelection() {
            PageGraphNode[] nodes = selection.OfType<PageGraphNode>().ToArray();
            for(int i = nodes.Length - 1; i >= 0; i--) {
                _scenario.DestroyPage(nodes[i].Page);
            }
            Rebuild();
            return EventPropagation.Stop;
        }

        public void DrawEdge() {
            Color color = CommonEditorResources.Instance.NodeArrowColor;
            Handles.color = color;

            foreach(PageGraphNode node in _nodes) {
                foreach(ScenarioPage referencingPage in node.Page.GetReferencingFamilyPages()) {
                    PageGraphNode targetNode = _nodes.Find(x => x.Page == referencingPage);
                    // 描画
                    Vector2 fromPos = node.Top * contentViewContainer.transform.scale.x + (Vector2)contentViewContainer.transform.position;
                    Vector2 toPos = targetNode.Bottom * contentViewContainer.transform.scale.x + (Vector2)contentViewContainer.transform.position;
                    Handles.DrawBezier(fromPos, toPos, fromPos + Vector2.up * 40, toPos + Vector2.down * 40, color, null, 2f);

                    float arrowRadius = 6;
                    float arrowHeight = 6;

                    Handles.DrawAAConvexPolygon(toPos, new Vector2(toPos.x - arrowRadius, toPos.y - arrowHeight), new Vector2(toPos.x + arrowRadius, toPos.y - arrowHeight));
                }
            }

            Handles.color = Color.white;
        }
    }
}