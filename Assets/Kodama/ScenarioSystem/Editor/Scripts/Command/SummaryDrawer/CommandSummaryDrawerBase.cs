using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public abstract class CommandSummaryDrawerBase {
        public virtual void Draw(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {

        }

        public virtual float GetHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}