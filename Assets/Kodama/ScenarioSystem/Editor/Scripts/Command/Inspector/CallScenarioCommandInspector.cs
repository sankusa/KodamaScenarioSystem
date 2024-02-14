using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomEditor(typeof(CallScenarioCommand), true)]
    public class CallScenarioCommandInspector : AsyncCommandBaseInspector {
        private ReorderableList _argList;
        public override void OnInspectorGUIMainForAsyncCommand() {
            SerializedProperty targetProp = serializedObject.FindProperty("_target");
            SerializedProperty callTypeProp = serializedObject.FindProperty("_callType");
            SerializedProperty scenarioArgsProp = serializedObject.FindProperty("_scenarioArgsProp");
            if(_argList == null)_argList = new ReorderableList(serializedObject, scenarioArgsProp);
            EditorGUILayout.PropertyField(targetProp);
            EditorGUILayout.PropertyField(callTypeProp);
            // EditorGUILayout.PropertyField(scenarioArgsProp);

            EditorGUILayout.LabelField("sssss");

            _argList.DoLayoutList();
        }
    }
}