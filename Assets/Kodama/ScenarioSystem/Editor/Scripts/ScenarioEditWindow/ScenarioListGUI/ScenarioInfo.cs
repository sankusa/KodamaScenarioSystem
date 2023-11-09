using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// シナリオアセットの表示用情報
    /// </summary>
    internal class ScenarioInfo {
        internal Scenario Scenario {get;}
        internal string Path {get;} 
        
        internal ScenarioInfo(string path) {
            Scenario = AssetDatabase.LoadAssetAtPath<Scenario>(path);
            Path = path;
        }
    }
}