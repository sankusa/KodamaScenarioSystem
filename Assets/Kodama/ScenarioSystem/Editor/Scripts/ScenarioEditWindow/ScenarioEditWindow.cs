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
        private static ScenarioEditWindow _instance;
        private Scenario _currentScenario;
        private SerializedObject _serializedObject;
        
        private ScenarioListGUI _scenarioListGUI;
        private ScenarioEditGUI _scenarioEditGUI;

        private ScenarioEditWindowStatus _windowStatus;
        
        [MenuItem(nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(ScenarioEditWindow))]
        public static void Open() {
            ScenarioEditWindow window = GetWindow<ScenarioEditWindow>("Scenario Edit");
            _instance = window;
            window._currentScenario = null;
            window.Initialize();
        }

        public static void OpenEditGUI(Scenario scenario) {
            ScenarioEditWindow window = GetWindow<ScenarioEditWindow>("Scenario Edit");
            _instance = window;
            window._currentScenario = scenario;
            window.InitializeEditGUI();
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
            if(_currentScenario == null) {
                Initialize();
            }
            else {
                InitializeEditGUI();
            }
            EditorApplication.projectChanged += OnProjectChanged;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDisable() {
            EditorApplication.projectChanged -= OnProjectChanged;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        void OnDestroy() {
            _instance = null;
        }

        private void Initialize() {
            rootVisualElement.Clear();

            _scenarioListGUI = new ScenarioListGUI();
            _scenarioEditGUI = null;// new ScenarioEditGUI(rootVisualElement);
            _serializedObject = null;
            _windowStatus = new ScenarioEditWindowStatus();
        }

        private void InitializeEditGUI() {
            rootVisualElement.Clear();

            _scenarioListGUI = new ScenarioListGUI();
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
            //     _scenarioEditGUI.DrawLayout(_windowStatus, _currentScenario, _serializedObject);
            // });
            // imguiContainer.style.left = 300;
            // imguiContainer.style.width = position.width - 500;
            // imguiContainer.style.top = 200;
            // imguiContainer.style.height = 200;
            // horizontalLayout.Add(imguiContainer);
        }

        private void OnUndoRedo() {
            Repaint();
        }

        private void OnProjectChanged() {
            _scenarioListGUI = new ScenarioListGUI();
        }

        void OnGUI() {
            if(_currentScenario == null) {
                _scenarioListGUI.DrawLayout(position);
            }
            else {
                if(_serializedObject == null) {
                    _serializedObject = new SerializedObject(_currentScenario);
                }

                _scenarioEditGUI.DrawLayout(_windowStatus, _currentScenario, _serializedObject);
            }
        }
    }
}