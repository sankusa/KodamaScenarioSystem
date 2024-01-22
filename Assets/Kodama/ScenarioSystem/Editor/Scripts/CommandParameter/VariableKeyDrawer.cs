using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(VariableKey<>), true)]
    public class VariableKeyDrawer : PropertyDrawer {
        private static readonly string[] _emptyIdArray = new string[]{""};
        private static readonly string[] _emptyNameArray = new string[]{Labels.Label_Empty};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            VariableKey variableKey = property.GetObject() as VariableKey;
            Type targetType = variableKey.TargetType;
            CommandBase command = property.serializedObject.targetObject as CommandBase;
            SerializedProperty idProp = property.FindPropertyRelative("_id");
            IEnumerable<VariableBase> variableDefines = command.GetAvailableVariableDefines().Where(x => x.TargetType == targetType);
            string[] idList = variableDefines.Select(x => x.Id).ToArray();
            string[] emptyAndIdList = _emptyIdArray.Concat(idList).ToArray();
            string[] names = variableDefines.Select(x => x.Name).ToArray();
            string[] emptyAndNames = _emptyNameArray.Concat(names).ToArray();

            int selectedIndex = Array.IndexOf(emptyAndIdList, idProp.stringValue);
            
            selectedIndex = EditorGUI.Popup(rect, label.text, selectedIndex, emptyAndNames);
            
            if(selectedIndex != -1) {
                idProp.stringValue = emptyAndIdList[selectedIndex];
            }
        }
    }
}