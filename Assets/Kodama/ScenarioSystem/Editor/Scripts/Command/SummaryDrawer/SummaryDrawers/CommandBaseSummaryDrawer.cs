using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected StringBuilder _sb = new StringBuilder();

        public override void Draw(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            float mainViewHeight = GetMainViewHeight(command, groupSetting, commandSetting);
            Rect mainViewRect = new Rect(rect) {height = mainViewHeight};
            DrawMainView(mainViewRect, command, groupSetting, commandSetting);

            float errorMessageViewHeight = GetErrorMessageViewHeight(command, groupSetting, commandSetting);
            Rect errorMessageViewRect = new Rect(rect) {y = rect.y + mainViewHeight, height = errorMessageViewHeight};
            DrawErrorMessageView(errorMessageViewRect, command, groupSetting, commandSetting);
        }

        protected virtual void DrawMainView(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            rect.xMin += 8;

            bool drawIcon = WillDrawIcon(command, groupSetting, commandSetting);
            // アイコン表示
            if(drawIcon) {
                Rect iconRect = new Rect(rect.x, rect.y, _iconWidthHeight, _iconWidthHeight);

                // 高速化のためScopeを使わずに色設定を変更
                Color originalBackgroundColor = GUI.contentColor;
                GUI.contentColor = commandSetting.IconColor;

                EditorGUI.LabelField(iconRect, new GUIContent(commandSetting.Icon, null), GUIStyle.none);

                GUI.contentColor = originalBackgroundColor;
            }

            float offsetXOfIcon = drawIcon ? _iconWidthHeight + _standardSpace : 0;
            rect.xMin += offsetXOfIcon;

            bool drawLabel = WillDrawLabel(command, groupSetting, commandSetting);
            float labelWidth = _labelWidth - offsetXOfIcon;
            if(drawLabel) {
                Rect labelRect = new Rect(rect.x, rect.y + _standardSpace, _labelWidth, _lineHeight);
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
            return commandSetting.HideDisplayName == false;
        }

        protected virtual void DrawLabel(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            Color textBaseColor = CommonEditorResources.Instance.SummaryTextColor;
            Color labelColor =
                groupSetting.CaptionColor * groupSetting.CaptionColor.a
                + textBaseColor * (1 -  groupSetting.CaptionColor.a);
            labelColor.a = 1;

            _sb.Append("<color=#");
            _sb.Append(ColorUtil.ToHtmlStringRGBA(labelColor));
            _sb.Append(">");
            _sb.Append(commandSetting.DisplayName);
            _sb.Append("</color>");
            string label = _sb.ToString();
            _sb.Clear();

            EditorGUI.LabelField(rect, label, GUIStyles.SummaryLabel);
        }

        protected virtual void DrawSummary(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            Color textBaseColor = CommonEditorResources.Instance.SummaryTextColor;

            _sb.Append("<color=#");
            _sb.Append(ColorUtil.ToHtmlStringRGBA(textBaseColor));
            _sb.Append(">");
            _sb.Append(command.GetSummary());
            _sb.Append("</color>");
            string summary = _sb.ToString();
            _sb.Clear();

            EditorGUI.LabelField(rect, summary, GUIStyles.SummaryLabel);            
        }

        protected virtual void DrawErrorMessageView(Rect rect, CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            string errorMessage = command.Validate();
            if(string.IsNullOrEmpty(errorMessage)) return;

            // rect.xMin += _standardSpace;
            // rect.xMax -= _standardSpace;

            EditorGUI.HelpBox(rect, errorMessage, MessageType.Error);
        }

        public override float GetHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            return GetMainViewHeight(command, groupSetting, commandSetting) +
            GetErrorMessageViewHeight(command, groupSetting, commandSetting);
        }

        protected virtual float GetMainViewHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            float contentHeight = 0;

            // サマリ行数
            string summary = command.GetSummary();
            int summaryRowCount;
            if(string.IsNullOrEmpty(summary)) {
                summaryRowCount = 1;
            }
            else {
                summaryRowCount = summary.CountLine();
            }

            // 行数分高さを加算
            float summaryHeight = summaryRowCount * _lineHeight;
            contentHeight += summaryHeight;

            // サマリ位置:下の場合の位置調整
            bool drawLabel = WillDrawLabel(command, groupSetting, commandSetting);
            if(drawLabel && commandSetting.SummaryPosition == SummaryPosition.Bottom) {
                contentHeight += _lineHeight + _multipleSummaryOffset.y;
            }

            return _standardSpace * 2 + contentHeight + 1;
        }

        protected virtual float GetErrorMessageViewHeight(CommandBase command, CommandGroupSetting groupSetting, CommandSetting commandSetting) {
            string errorMessage = command.Validate();
            if(string.IsNullOrEmpty(errorMessage)) return 0;
            return errorMessage.CountLine() * _lineHeight + _standardSpace * 2;
        }
    }
}