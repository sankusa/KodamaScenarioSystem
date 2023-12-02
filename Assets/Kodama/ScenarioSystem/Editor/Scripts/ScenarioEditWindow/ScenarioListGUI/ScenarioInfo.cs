using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// シナリオアセットの表示用情報
    /// </summary>
    internal class ScenarioInfo {
        public Scenario Scenario {get;}
        public string Path {get;} 
        
        public ScenarioInfo(string path) {
            Scenario = AssetDatabase.LoadAssetAtPath<Scenario>(path);
            Path = path;
        }
    }
}