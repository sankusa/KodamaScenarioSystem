using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CommandSummaryDrawer(typeof(AsyncCommandBase))]
    public class AsyncCommandBaseSummaryDrawer : CommandBaseSummaryDrawer {
        protected override void DrawMainView(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            AsyncCommandBase asyncCommand = command as AsyncCommandBase;
            if(asyncCommand.AsyncCommandSetting.Wait == false && asyncCommand.AsyncCommandSetting.SetUniTaskTo.IsEmpty() == false) {
                Rect mainViewRect = new Rect(rect) {yMax = rect.yMax - EditorGUIUtility.singleLineHeight};
                base.DrawMainView(mainViewRect, command, groupSetting, commandSetting);

                Rect waitSettingRect = new Rect(rect) {yMin = rect.yMin + mainViewRect.height};

                Color textBaseColor = CommonEditorResources.Instance.SummaryTextColor;
                _sb.Append("    <color=#");
                _sb.Append(ColorUtil.ToHtmlStringRGBA(textBaseColor));
                _sb.Append(">Set UniTask -> ");
                _sb.Append(asyncCommand.AsyncCommandSetting.SetUniTaskTo.GetSummary(command));
                _sb.Append("</color>");
                string summary = _sb.ToString();
                _sb.Clear();

                EditorGUI.DrawRect(waitSettingRect, new Color(0, 0, 0, 0.2f));
                EditorGUI.LabelField(waitSettingRect, summary, GUIStyles.SummaryLabel);
            }
            else {
                base.DrawMainView(rect, command, groupSetting, commandSetting);
            }

            Color waitColor;
            if(asyncCommand.AsyncCommandSetting.Wait) {
                waitColor = new Color(1, 1, 1, 0.15f);
            }
            else {
                waitColor = Color.black;
            }
            EditorGUI.DrawRect(new Rect(rect.xMin + 2, rect.y, 2, rect.height), waitColor);
        }
        
        protected override float GetMainViewHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            float standardHeight = base.GetMainViewHeight(command, groupSetting, commandSetting);

            AsyncCommandBase asyncCommand = command as AsyncCommandBase;
            if(asyncCommand.AsyncCommandSetting.Wait == false && asyncCommand.AsyncCommandSetting.SetUniTaskTo.IsEmpty() == false) {
                return standardHeight + EditorGUIUtility.singleLineHeight;
            }
            return standardHeight;
        }
    }
}