using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [Serializable]
    public class CommandGroupSetting {
        [SerializeField] private string _groupId;
        [SerializeField] private string _displayName;
        [SerializeField] private Color _groupColor = Color.white;

        public string GroupId => _groupId;
        public string DisplayName => _displayName;
        public Color GroupColor => _groupColor;
    }
}