using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kodama.ScenarioSystem {
    [Serializable]
    internal class CommandSetting
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _groupId;
        [SerializeField] private Texture2D _icon;
        [SerializeField] private MonoScript _commandScript;

        public string DisplayName => _displayName;
        public string GroupId => _groupId;
        public Texture2D Icon => _icon;
        public MonoScript CommandScript => _commandScript;
    }
}