using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditWindow : EditorWindow {
        private Scenario _currentScenario;
        private SerializedObject _serializedObject;
        
        private ScenarioEditGUI _scenarioEditGUI;

        private ScenarioEditWindowStatus _windowStatus;
        
        public static void OpenEditGUI(Scenario scenario) {
            ScenarioEditWindow window = GetWindow<ScenarioEditWindow>("Scenario Edit");
            window._currentScenario = scenario;
            window.Initialize();
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line) {
            if(EditorUtility.InstanceIDToObject(instanceId) is Scenario scenario) {
                OpenEditGUI(scenario);
                return true;
            }

            return false;
        }

        void OnEnable() {
            Initialize();

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDisable() {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void Initialize() {
            rootVisualElement.Clear();

            _scenarioEditGUI = new ScenarioEditGUI(_currentScenario, rootVisualElement);
            _serializedObject = new SerializedObject(_currentScenario);
            _windowStatus = new ScenarioEditWindowStatus();

            // VisualElement horizontalLayout = new VisualElement();
            // var flexDirection = horizontalLayout.style.flexDirection;
            // flexDirection.value = FlexDirection.Row;

            // rootVisualElement.Add(horizontalLayout);

            // PageGraphView pageGraphView = new PageGraphView();
            // pageGraphView.style.left = 0;
            // pageGraphView.style.width = 200;
            // pageGraphView.style.height = 200;
            // horizontalLayout.Add(pageGraphView);

            // IMGUIContainer imguiContainer = new IMGUIContainer(() => {
            //     if(_currentScenario == null) return;

            //     // if(_serializedObject == null) {
            //     //     _serializedObject = new SerializedObject(_currentScenario);
            //     // }

            //     _scenarioEditGUI.DrawLayout(position, _windowStatus, _currentScenario, _serializedObject);

            //     // フィールドに入力した文字を変換確定後、Repaintが走るまで変換候補の表示が残るため
            //     if(Event.current.type == EventType.KeyUp) {
            //         Repaint();
            //     }
            // });
            // imguiContainer.style.left = 300;
            // imguiContainer.style.width = position.width - 500;
            // imguiContainer.style.top = 200;
            // imguiContainer.style.height = 200;
            // rootVisualElement.Add(imguiContainer);
        }

        private void OnUndoRedo() {
            Repaint();
        }

        void OnGUI() {
            if(_currentScenario == null) return;

            _scenarioEditGUI.DrawLayout(position, _windowStatus, _currentScenario, _serializedObject);

            // フィールドに入力した文字を変換確定後、Repaintが走るまで変換候補の表示が残るため
            if(Event.current.type == EventType.KeyUp) {
                Repaint();
            }
        }
    }
}