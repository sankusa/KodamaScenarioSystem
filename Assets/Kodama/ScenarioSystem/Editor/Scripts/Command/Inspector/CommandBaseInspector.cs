using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(CommandBase), true)]
    public class CommandBaseInspector : CommandInspectorBase {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            OnInspectorGUIMain();

            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public virtual void OnInspectorGUIMain() {
            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true);
            do {
                if(property.name == "m_Script") continue;
                EditorGUILayout.PropertyField(property);
            } while(property.NextVisible(false));
        }
    }
}