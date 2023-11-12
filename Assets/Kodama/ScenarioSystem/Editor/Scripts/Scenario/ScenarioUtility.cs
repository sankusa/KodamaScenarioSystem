using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public static class ScenarioUtility {
        public static void CreateScenario() {
            string path = EditorUtility.SaveFilePanel("", "Assets", "Scenario", "asset");

            if(!string.IsNullOrEmpty(path)) {
                Scenario scenario = ScriptableObject.CreateInstance<Scenario>();
                AssetDatabase.CreateAsset(scenario, path.Replace(Application.dataPath, "Assets"));
            }
        }
    }
}