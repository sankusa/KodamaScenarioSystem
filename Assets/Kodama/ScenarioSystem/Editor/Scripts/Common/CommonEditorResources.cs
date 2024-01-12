using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CreateAssetMenu(fileName = nameof(CommonEditorResources), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(CommonEditorResources))]
    public class CommonEditorResources : ScriptableObject {
        private static CommonEditorResources _instance;
        public static CommonEditorResources Instance => _instance;

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.projectChanged += OnProjectChanged;
            LoadAsset();
        }

        private static void OnProjectChanged() {
            LoadAsset();
        }

        private static void LoadAsset() {
            _instance = AssetUtility.LoadAllAssets<CommonEditorResources>().FirstOrDefault();
        }
        [SerializeField] private Texture2D _commandAddIcon;
        public Texture2D CommandAddIcon => _commandAddIcon;

        [SerializeField] private Texture2D _commandInsertIcon;
        public Texture2D CommandInsertIcon => _commandInsertIcon;

        [SerializeField] private Texture2D _commandDeleteIcon;
        public Texture2D CommandDeleteIcon => _commandDeleteIcon;

        [SerializeField] private Texture2D _commandCopyIcon;
        public Texture2D CommandCopyIcon => _commandCopyIcon;

        [SerializeField] private Texture2D _menuIcon;
        public Texture2D MenuIcon => _menuIcon;
    }
}