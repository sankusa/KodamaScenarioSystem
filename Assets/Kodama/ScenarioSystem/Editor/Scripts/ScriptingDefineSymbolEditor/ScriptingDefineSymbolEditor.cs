using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class ScriptingDefineSymbolEditor : EditorWindow {
        private class SymbolStatus {
            private readonly string _symbol;
            public string Symbol => _symbol;
            private bool _valid;
            public bool Valid {
                get => _valid;
                set => _valid = value;
            }

            public SymbolStatus(string symbol) {
                _symbol = symbol;
            }
        }

        private string[] _targetSymbols = new string[]{"KODAMA_SCENARIO_ADDRESSABLE_SUPPORT", "KODAMA_SCENARIO_ZENJECT_SUPPORT"};
        private Vector2 _scrollPos;

        private SymbolStatus[] _symbolStatuses;

        [MenuItem(nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(ScriptingDefineSymbolEditor))]
        private static void Open() {
            GetWindow<ScriptingDefineSymbolEditor>();
        }

        void OnEnable() {
            _symbolStatuses = _targetSymbols.Select(x => new SymbolStatus(x)).ToArray();
            LoadSymbolStatuses();
        }

        private void LoadSymbolStatuses() {
            string[] definedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            foreach(SymbolStatus symbolStatus in _symbolStatuses) {
                symbolStatus.Valid = definedSymbols.Contains(symbolStatus.Symbol);
            }
        }

        private void SaveSymbolStatuses() {
            List<string> symbols = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup
                )
                .Split(';')
                .ToList();

            foreach(SymbolStatus symbolStatus in _symbolStatuses) {
                if(symbolStatus.Valid) {
                    if(symbols.Contains(symbolStatus.Symbol) == false) {
                        symbols.Add(symbolStatus.Symbol);
                    }
                }
                else {
                    if(symbols.Contains(symbolStatus.Symbol)) {
                        symbols.Remove(symbolStatus.Symbol);
                    }
                }
            }

            PlayerSettings
                .SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", symbols)
                );
        }

        void OnGUI() {
            EditorGUILayout.BeginScrollView(_scrollPos);

            foreach(SymbolStatus symbolStatus in _symbolStatuses) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(symbolStatus.Symbol);
                symbolStatus.Valid = EditorGUILayout.Toggle(symbolStatus.Valid, GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Apply")) {
                SaveSymbolStatuses();
            }

            EditorGUILayout.EndScrollView();
        }


    }
}