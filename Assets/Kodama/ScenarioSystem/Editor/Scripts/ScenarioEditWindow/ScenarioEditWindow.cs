using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditWindow : EditorWindow {
        private static ScenarioEditWindow _instance;
        public static ScenarioEditWindow Instance => _instance;
        private Scenario _currentScenario;
        private SerializedObject _serializedObject;
        
        private ScenarioListGUI _scenarioListGUI;
        private ScenarioEditGUI _scenarioEditGUI;

        private ScenarioEditWindowStatus _windowStatus;
        
        [MenuItem(nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(ScenarioEditWindow))]
        internal static void Open() {
            ScenarioEditWindow window = GetWindow<ScenarioEditWindow>("Scenario Edit");
            _instance = window;
            window.Initialize();
            window._currentScenario = null;
        }

        internal static void OpenEditGUI(Scenario scenario) {
            ScenarioEditWindow window = GetWindow<ScenarioEditWindow>("Scenario Edit");
            _instance = window;
            window.Initialize();
            window._currentScenario = scenario;
        }

        void OnEnable() {
            Initialize();
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
            _scenarioListGUI = new ScenarioListGUI();
            _scenarioEditGUI = new ScenarioEditGUI();
            _serializedObject = null;
            _windowStatus = new ScenarioEditWindowStatus();
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