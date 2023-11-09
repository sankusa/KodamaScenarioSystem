using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

namespace Kodama.ScenarioSystem.Editor {
    //[CustomPropertyDrawer(typeof(CommandBase), true)]
    public class SomeClassDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            int depth = property.depth;
            property.NextVisible(true);
            do {
                if(property.depth != depth + 1) break;
                EditorGUILayout.PropertyField(property);
            } while(property.NextVisible(false));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}