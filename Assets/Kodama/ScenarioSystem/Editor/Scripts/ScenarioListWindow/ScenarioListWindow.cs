using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class ScenarioListWindow : EditorWindow {

        private ScenarioListGUI _scenarioListGUI;

        [MenuItem(nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(ScenarioListWindow))]
        public static void Open() {
            GetWindow<ScenarioListWindow>("Scenario List");
        }

        void OnEnable() {
            EditorApplication.projectChanged += OnProjectChanged;

            Initialize();
        }

        void OnDisable() {
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void OnProjectChanged() {
            Initialize();
        }

        private void Initialize() {
            _scenarioListGUI = new ScenarioListGUI();
        }
        
        void OnGUI() {
            _scenarioListGUI.DrawLayout(position);
        }
    }
}