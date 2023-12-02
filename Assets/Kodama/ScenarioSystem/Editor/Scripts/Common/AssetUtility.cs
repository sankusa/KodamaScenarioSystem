using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Kodama.ScenarioSystem.Editor {
    internal static class AssetUtility {
        /// <summary>
        /// プロジェクト内の対象型のアセットを全てロード
        /// </summary>
        public static List<T> LoadAllAssets<T>() where T : Object {
            List<T> list = new List<T>(); 
            
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            foreach(string guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
                list.Add(asset);
            }
            return list;
        }

        public static IEnumerable<string> GetAllAssetPaths<T>() where T : Object {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(AssetDatabase.GUIDToAssetPath);
        }
    }
}