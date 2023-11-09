using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [Serializable]
    public class VariableSetting {
        [SerializeField] private string _displayName;
        public string DisplayName => _displayName;
        [SerializeField] private string _typeFullName;
        public string TypeFullname => _typeFullName;
        [SerializeField] private MonoScript _nonGenericVariableScript;
        public MonoScript NonGenericVariableScript => _nonGenericVariableScript;
    }

    [CreateAssetMenu(fileName = nameof(VariableSettingTable), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(VariableSettingTable))]
    public class VariableSettingTable : ScriptableObject {
        private static List<VariableSettingTable> tables;
        internal static IEnumerable<VariableSetting> AllSettings => tables.SelectMany(x => x.Settings);

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.projectChanged += OnProjectChanged;
            LoadAllAssets();
        }

        private static void OnProjectChanged() {
            LoadAllAssets();
        }

        private static void LoadAllAssets() {
            tables = AssetUtility.LoadAllAssets<VariableSettingTable>();
        }

        [SerializeField] private List<VariableSetting> _settings;
        public IReadOnlyList<VariableSetting> Settings => _settings;
    }
}