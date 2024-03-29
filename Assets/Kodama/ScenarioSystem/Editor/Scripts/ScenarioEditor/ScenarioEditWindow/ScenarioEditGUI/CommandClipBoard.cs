using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    public static class CommandClipBoard {
        private static List<CommandBase> _commands = new List<CommandBase>();
        public static bool Any => _commands.Any();
        public static int Count => _commands.Count;

        public static void CopyToClipBoard(IEnumerable<CommandBase> commands) {
            _commands.Clear();
            _commands.AddRange(commands.Select(x => x.Copy(null)));
        }

        public static List<CommandBase> CopyFromClipBoardWithUndo(ScenarioPage overwriteParentPage) {
            return _commands.Select(x => x.CopyWithUndo(overwriteParentPage)).ToList();
        }

        public static void Clear() {
            _commands.Clear();
        }
    }
}