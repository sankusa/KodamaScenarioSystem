using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CommandSummaryDrawer(typeof(CommandBase))]
    public class CommandBaseSummaryDrawer : CommandSummaryDrawerBase {
        private const float _labelWidth = 140;
        private const float _lineHeight = 15;
        private const float _iconWidthHeight = 24;
        private const float _standardSpace = 4;
        private static readonly Vector2 _multipleSummaryOffset = new Vector2(10, 2);

        public override void Draw(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            rect.xMin += 8;

            bool drawIcon = WillDrawIcon(command, groupSetting, commandSetting);
            // アイコン表示
            if(drawIcon) {
                Rect iconRect = new Rect(rect.x, rect.y, _iconWidthHeight, _iconWidthHeight);
                using (new ContentColorScope(commandSetting.IconColor)) {
                    EditorGUI.LabelField(iconRect, new GUIContent(commandSetting.Icon, null), GUIStyle.none);
                }
            }

            float offsetXOfIcon = drawIcon ? _iconWidthHeight + _standardSpace : 0;
            rect.xMin += offsetXOfIcon;

            bool drawLabel = WillDrawLabel(command, groupSetting, commandSetting);
            float labelWidth = _labelWidth - offsetXOfIcon;
            if(drawLabel) {
                Rect labelRect = new Rect(rect.x, rect.y + _standardSpace + 1, _labelWidth, _lineHeight);
                DrawLabel(labelRect, command, groupSetting, commandSetting);
            }
            
            Rect summaryRect;
            if(drawLabel) {
                if(commandSetting.SummaryPosition == SummaryPosition.Bottom) {
                    summaryRect = new Rect(rect.x + _multipleSummaryOffset.x, rect.y + _lineHeight + _multipleSummaryOffset.y, rect.width - _multipleSummaryOffset.x, rect.height - _lineHeight);
                }
                else {
                    summaryRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
                }
            } else {
                summaryRect = new Rect(rect.x + offsetXOfIcon, rect.y, rect.width, rect.height);
            }
            DrawSummary(summaryRect, command, groupSetting, commandSetting); 
        }

        protected virtual bool WillDrawIcon(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            return commandSetting.Icon != null;
        }

        protected virtual bool WillDrawLabel(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            return true;
        }

        protected virtual void DrawLabel(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            Color textBaseColor = CommonEditorResources.Instance.SummaryTextColor;
            Color labelColor =
                groupSetting.CaptionColor * groupSetting.CaptionColor.a
                + textBaseColor * (1 -  groupSetting.CaptionColor.a);
            labelColor.a = 1;
            string labelColorString = ColorUtility.ToHtmlStringRGBA(labelColor);
            EditorGUI.LabelField(rect, $"<color=#{labelColorString}>{commandSetting.DisplayName}</color>", GUIStyles.SummaryLabel);
        }

        protected virtual void DrawSummary(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            Color textBaseColor = CommonEditorResources.Instance.SummaryTextColor;
            string textBaseColorString = ColorUtility.ToHtmlStringRGBA(textBaseColor);
            EditorGUI.LabelField(rect, $"<color=#{textBaseColorString}>{command.GetSummary()}</color>", GUIStyles.SummaryLabel);
        }

        public override float GetHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            int summaryRowCount = 1 + command.GetSummary().Where(c => c == '\n').Count();
            bool drawLabel = WillDrawLabel(command, groupSetting, commandSetting);
            float summaryHeight = summaryRowCount * _lineHeight;
            float adjustForSummaryPosition = commandSetting.SummaryPosition == SummaryPosition.Bottom
                ? (drawLabel ? _lineHeight + _multipleSummaryOffset.y : 0)
                : 0;
            return _standardSpace * 2 + summaryHeight + adjustForSummaryPosition - 1;
        }
    }
}