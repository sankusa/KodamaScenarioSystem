using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [InitializeOnLoad]
    public static class ScenarioSaver {
        static ScenarioSaver() {
            EditorApplication.playModeStateChanged += state => {
                if(EditorApplication.isPlayingOrWillChangePlaymode == false) return;
                AssetUtility.LoadAllAssets<Scenario>().ForEach(x => AssetDatabase.SaveAssetIfDirty(x));
            };
        }
    }
}