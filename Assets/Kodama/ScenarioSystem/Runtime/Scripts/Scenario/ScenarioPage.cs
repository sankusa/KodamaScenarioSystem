using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kodama.ScenarioSystem
{
    [Serializable]
    public class ScenarioPage : ScriptableObject {
        [SerializeField] private Scenario _scenario;
        public Scenario Scenario {
            get => _scenario;
            set {
                Assert.IsTrue(_scenario == null);
                _scenario = value;
            }
        }
        [SerializeReference] private List<CommandBase> _commands = new List<CommandBase>();
        public IReadOnlyList<CommandBase> Commands => _commands;

        public void AddCommand(CommandBase command) {
            _commands.Add(command);
        }

        public void InsertCommand(int index, CommandBase command) {
            _commands.Insert(index, command);
        }

        public bool RemoveCommand(CommandBase command) {
            return _commands.Remove(command);
        }

        public void RemoveCommandAt(int index) {
            _commands.RemoveAt(index);
        }

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
    }
}