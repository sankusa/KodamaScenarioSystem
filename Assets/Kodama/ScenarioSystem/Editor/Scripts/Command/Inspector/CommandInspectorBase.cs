using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    public class CommandInspectorBase : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();

            SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
            (CommandGroupSetting groupSetting, CommandSetting commandSetting) settings = CommandGroupSetting.Find(target as CommandBase);
            scriptProp.isExpanded = HeaderFoldOut.BeginLayoutFoldoutGroup(settings.commandSetting.DisplayName, scriptProp.isExpanded, settings.groupSetting.Color);
            if(scriptProp.isExpanded) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(scriptProp);
                EditorGUI.EndDisabledGroup();
            }
            HeaderFoldOut.EndLayoutFoldoutGroup();
        }
    }
}