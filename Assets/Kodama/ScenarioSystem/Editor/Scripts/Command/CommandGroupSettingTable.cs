using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CreateAssetMenu(fileName = "New " + nameof(CommandGroupSettingTable), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(CommandGroupSettingTable))]
    internal class CommandGroupSettingTable : ScriptableObject {
        private static List<CommandGroupSettingTable> tables;
        internal static IEnumerable<CommandGroupSetting> AllSettings => tables.SelectMany(x => x.Settings);

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.projectChanged += OnProjectChanged;
            LoadAllAssets();
        }

        private static void OnProjectChanged() {
            LoadAllAssets();
        }

        private static void LoadAllAssets() {
            tables = AssetUtility.LoadAllAssets<CommandGroupSettingTable>();
        }

        [SerializeField] private List<CommandGroupSetting> _settings;
        internal IReadOnlyList<CommandGroupSetting> Settings => _settings;
    }
}