using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public static class CommandClipBoard {
        private static List<CommandBase> _commands = new List<CommandBase>();
        public static bool Any => _commands.Any();
        public static int Count => _commands.Count;

        public static void CopyToClipBoard(IEnumerable<CommandBase> commands) {
            _commands.Clear();
            _commands.AddRange(commands.Select(x => x.Copy(null)));
        }

        public static List<CommandBase> CopyFromClipBoard(ScenarioPage page) {
            return _commands.Select(x => x.Copy(page)).ToList();
        }

        public static void Clear() {
            _commands.Clear();
        }
    }
}