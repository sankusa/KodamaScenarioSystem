using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(AsyncCommandBase), true)]
    public class AsyncCommandBaseInspector : CommandBaseInspector {
        public override void OnInspectorGUIMain() {
            SerializedProperty asyncCommandSettingProp = serializedObject.FindProperty("_asyncCommandSetting");
            EditorGUILayout.PropertyField(asyncCommandSettingProp);

            OnInspectorGUIMainForAsyncCommand();
        }

        public virtual void OnInspectorGUIMainForAsyncCommand() {
            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true);
            do {
                if(property.name == "m_Script" || property.name == "_asyncCommandSetting") continue;
                EditorGUILayout.PropertyField(property);
            } while(property.NextVisible(false));
        }
    }
}