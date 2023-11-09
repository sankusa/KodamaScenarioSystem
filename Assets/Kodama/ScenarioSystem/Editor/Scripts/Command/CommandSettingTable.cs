using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Kodama.ScenarioSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [CreateAssetMenu(fileName = "New " + nameof(CommandSettingTable), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(CommandSettingTable))]
    internal class CommandSettingTable : ScriptableObject {
        private static List<CommandSettingTable> tables;
        internal static IEnumerable<CommandSetting> AllSettings => tables.SelectMany(x => x.Settings);

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.projectChanged += OnProjectChanged;
            LoadAllAssets();
        }

        private static void OnProjectChanged() {
            LoadAllAssets();
        }

        private static void LoadAllAssets() {
            tables = AssetUtility.LoadAllAssets<CommandSettingTable>();
        }

        [SerializeField] private List<CommandSetting> _settings = new List<CommandSetting>();
        internal IReadOnlyList<CommandSetting> Settings => _settings;
    }
}