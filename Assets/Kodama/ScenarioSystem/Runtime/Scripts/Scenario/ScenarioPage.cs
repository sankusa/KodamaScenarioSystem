using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ScenarioPage : ScriptableObject {
        [SerializeField] private Scenario _parentScenario;
        public Scenario ParentScenario {
            get => _parentScenario;
            set {
                Assert.IsTrue(_parentScenario == null);
                _parentScenario = value;
            }
        }
        [SerializeField] private List<CommandBase> _commands = new List<CommandBase>();
        public IReadOnlyList<CommandBase> Commands => _commands;

#if UNITY_EDITOR
        /// <summary>
        /// For Graph View
        /// </summary>
        /// <param name="command"></param>
        [SerializeField] private Vector2 _nodePosition;
        public Vector2 NodePosition {
            get => _nodePosition;
            set => _nodePosition = value;
        }

        private const string _undoRedoLabel_AddCommand = "Add Command";
        private const string _undoRedoLabel_RemoveCommand = "Remove Command";

        public void AddCommand(CommandBase command) {
            Undo.RecordObject(this, _undoRedoLabel_AddCommand);
            _commands.Add(command);
            AssetDatabase.AddObjectToAsset(command, this);
        }

        public void InsertCommand(int index, CommandBase command) {
            int insertIndex = Mathf.Clamp(index, 0, Commands.Count);
            Undo.RecordObject(this, _undoRedoLabel_AddCommand);
            _commands.Insert(insertIndex, command);
            AssetDatabase.AddObjectToAsset(command, this);
        }

        public void InsertCommands(int index, IEnumerable<CommandBase> commands) {
            int insertIndex = Mathf.Clamp(index, 0, Commands.Count);
            Undo.RecordObject(this, _undoRedoLabel_AddCommand);
            _commands.InsertRange(insertIndex, commands);
            foreach(CommandBase command in commands) {
                AssetDatabase.AddObjectToAsset(command, this);
            }
        }

        public bool RemoveCommand(CommandBase command) {
            Undo.RecordObject(this, _undoRedoLabel_RemoveCommand);
            bool ret = _commands.Remove(command);
            Undo.DestroyObjectImmediate(command);
            return ret;
        }

        public void RemoveCommandAt(int index) {
            CommandBase command = _commands[index];
            Undo.RecordObject(this, _undoRedoLabel_RemoveCommand);
            _commands.RemoveAt(index);
            Undo.DestroyObjectImmediate(command);
        }
#endif

        public int IndexOf(CommandBase command) {
            return _commands.IndexOf(command);
        }

        public int FindBlockEndIndex(IBlockStart blockStart) {
            int startIndex = _commands.IndexOf(blockStart as CommandBase);
            if(startIndex == -1) {
                Debug.LogWarning($"blockStart not found in page.");
                return _commands.Count;
            }

            Stack<string> blockTypeLabelStack = new Stack<string>();
            blockTypeLabelStack.Push(blockStart.BlockType);
            for(int i = startIndex + 1; i < _commands.Count; i++) {
                if(_commands[i] is IBlockEnd end) {
                    if(blockTypeLabelStack.Peek() == end.BlockType) {
                        blockTypeLabelStack.Pop();
                        if(blockTypeLabelStack.Count == 0) {
                            return i;
                        }
                    }
                }

                if(_commands[i] is IBlockStart start) {
                    blockTypeLabelStack.Push(start.BlockType);
                }
            }

            return _commands.Count;
        }

        public bool IsSiblig(ScenarioPage page) {
            return ParentScenario.Pages.Contains(page);
        }

        public IEnumerable<ScenarioPage> GetReferencingSiblingPages() {
            return Commands
                .Where(x => x is CallPageCommand)
                .Select(x => (x as CallPageCommand).TargetPage)
                .Where(x => x != null)
                .Where(x => IsSiblig(x))
                .Distinct();
        }
    }
}