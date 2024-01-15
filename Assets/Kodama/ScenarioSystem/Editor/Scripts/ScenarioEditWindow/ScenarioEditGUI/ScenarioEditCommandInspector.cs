using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kodama.ScenarioSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ScenarioEditCommandInspector {
        public void DrawLayout(SerializedProperty commandProp) {
            // フレームのみ先に描画
            // using(new EditorGUILayout.VerticalScope(GUIStyles.TitleBar)) {
            //     EditorGUILayout.LabelField("Command Inspector");
            // }
            Rect rect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            if(commandProp == null) return;

            // Commandsの要素削除時、SerializedObjectに削除が反映される前にこの関数が呼び出される場合がある
            // この場合propertyPathを利用したインスタンス取得(GetObject)はインデックスのずれた要素を取得
            // してしまうので早期リターン
            if(commandProp.serializedObject.FindProperty("_commands").arraySize
                != (commandProp.serializedObject.targetObject as ScenarioPage).Commands.Count) {
                return;
            }

            CommandBase command = commandProp.GetObject() as CommandBase;
            if(command == null) return;

            // ヘッダ
            Rect headerRect = new Rect(rect){
                xMin = rect.xMin + 4,
                xMax = rect.xMax - 4,
                yMin = rect.yMin + 2,
                height = EditorGUIUtility.singleLineHeight
            };
            Rect headerLabelRect = new Rect(headerRect){x = headerRect.x + 4};

            GUI.Box(headerRect, "", GUIStyles.LeanGroupBox);

            (CommandGroupSetting commandGroupSetting, CommandSetting commandSetting) =
                CommandGroupSetting.Find(command);
            if(commandSetting != null) EditorGUI.LabelField(headerLabelRect, $"<b>{commandSetting.DisplayName}</b>", GUIStyles.SummaryLabel);

            if(command is AsyncCommandBase) {
                SerializedProperty waitProp = commandProp.FindPropertyRelative("_wait");
                if(waitProp != null) {
                    Rect waitLabelRect = new Rect(headerRect) {x = headerRect.xMax - 60, xMax = headerRect.xMax - 20};
                    Rect waitToggleRect = new Rect(headerRect) {x = headerRect.xMax - 20};
                    EditorGUI.LabelField(waitLabelRect, "Await");
                    EditorGUI.PropertyField(waitToggleRect, waitProp, GUIContent.none);
                }
            }

            // PropertyField
            // ※待機のトグル→PropertyFieldnの順で描画しないとトグルから値の変更ができない
            Rect propertyDrawerRect = new Rect(rect) {
                xMin = rect.xMin + 6,
                xMax = rect.xMax - 4,
                y = rect.y + 24,
                // yMax = rect.yMax,
            };
            if(commandProp != null && propertyDrawerRect.height > 0) {
                EditorGUI.PropertyField(propertyDrawerRect, commandProp, GUIContent.none, true);
            }
        }
    }
}