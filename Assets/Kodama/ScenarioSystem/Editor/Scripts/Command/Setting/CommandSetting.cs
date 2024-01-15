using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kodama.ScenarioSystem.Editor {
    public enum SummaryPosition {
        Right,
        Bottom,
    }

    [Serializable]
    public class CommandSetting {
        [SerializeField] private MonoScript _commandScript;
        [SerializeField] private string _displayName;
        [Header("Display Style")]
        [SerializeField] private Texture2D _icon;
        [SerializeField] private Color _iconColor;
        [SerializeField] private SummaryPosition _summaryPosition;

        public MonoScript CommandScript => _commandScript;
        public string DisplayName => _displayName;
        public Texture2D Icon => _icon;
        public Color IconColor => _iconColor;
        public SummaryPosition SummaryPosition => _summaryPosition;
    }
}