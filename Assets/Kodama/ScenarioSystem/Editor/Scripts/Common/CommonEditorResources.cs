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

        [SerializeField] private Color _backgroundColor;
        public Color BackgroundColor => _backgroundColor;

        [SerializeField] private Color _summaryTextColor;
        public Color SummaryTextColor => _summaryTextColor;

        [SerializeField] private Color _entryNodeColor;
        public Color EntryNodeColor => _entryNodeColor;

        [SerializeField] private Color _nodeArrowColor;
        public Color NodeArrowColor => _nodeArrowColor;

        [SerializeField] private Color _nodeSelectionBorder;
        public Color NodeSelectionBorder => _nodeSelectionBorder;

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