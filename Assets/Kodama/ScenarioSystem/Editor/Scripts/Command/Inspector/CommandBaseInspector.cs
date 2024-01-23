using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(CommandBase), true)]
    public class CommandBaseInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

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

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true);
            do {
                if(property.name == "m_Script") continue;
                EditorGUILayout.PropertyField(property);
            } while(property.NextVisible(false));

            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}