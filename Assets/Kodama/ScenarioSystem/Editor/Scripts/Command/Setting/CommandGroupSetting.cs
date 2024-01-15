using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CreateAssetMenu(fileName = "New " + nameof(CommandGroupSetting), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(CommandGroupSetting))]
    public class CommandGroupSetting : ScriptableObject {
        private static List<CommandGroupSetting> _all;
        public static IEnumerable<CommandGroupSetting> All => _all.OrderBy(x => x.Priority);

        public static (CommandGroupSetting, CommandSetting) Find(CommandBase command) {
            foreach(CommandGroupSetting groupSetting in All) {
                foreach(CommandSetting commandSetting in groupSetting.CommandSettings) {
                    if(commandSetting.CommandScript?.GetClass() == command.GetType()) return (groupSetting, commandSetting);
                }
            }
            return (null, null);
        }

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.projectChanged += OnProjectChanged;
            LoadAllAssets();
        }

        private static void OnProjectChanged() {
            LoadAllAssets();
        }

        private static void LoadAllAssets() {
            _all = AssetUtility.LoadAllAssets<CommandGroupSetting>();
        }

        [SerializeField] private int _priority;
        [SerializeField] private string _displayName;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Color _captionColor = Color.white;
        [SerializeField] private List<CommandSetting> _commandSettings = new List<CommandSetting>();

        public int Priority => _priority;
        public string DisplayName => _displayName;
        public Color Color => _color;
        public Color CaptionColor => _captionColor;
        public IReadOnlyList<CommandSetting> CommandSettings => _commandSettings;
    }
}