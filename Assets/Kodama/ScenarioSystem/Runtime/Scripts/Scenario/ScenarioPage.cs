using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem
{
    [Serializable]
    public class ScenarioPage
    {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeReference] private List<CommandBase> _commands = new List<CommandBase>();
        public IList<CommandBase> Commands => _commands;

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