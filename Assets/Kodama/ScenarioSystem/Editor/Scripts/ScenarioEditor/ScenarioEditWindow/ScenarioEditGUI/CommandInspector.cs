using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kodama.ScenarioSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor.ScenarioEditor {
    public class CommandInspector {
        private CommandBase _commandOld;
        private UnityEditor.Editor _commandEditor;
        public void DrawLayout(SerializedProperty commandProp) {
            // フレームのみ先に描画
            if(commandProp == null) {
                _commandOld = null;
                return;
            }

            CommandBase command = commandProp.objectReferenceValue as CommandBase;

            // Commandsの要素削除時、SerializedObjectに削除が反映される前にこの関数が呼び出される場合がある
            // この場合propertyPathを利用したインスタンス取得(GetObject)はインデックスのずれた要素を取得
            // してしまうので早期リターン
            if(commandProp.serializedObject.FindProperty("_commands").arraySize
                != (commandProp.serializedObject.targetObject as ScenarioPage).Commands.Count) {
                return;
            }

            if(command != _commandOld) {
                _commandEditor = UnityEditor.Editor.CreateEditor(command);
            }
            _commandEditor.OnInspectorGUI();

            _commandOld = command;
        }
    }
}