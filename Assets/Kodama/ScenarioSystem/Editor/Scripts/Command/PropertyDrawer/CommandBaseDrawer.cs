using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(CommandBase), true)]
    public class CommandBaseDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.hasVisibleChildren == false) return;
            property.NextVisible(true);
            int depth = property.depth;
            float positionY = position.y;
            do {
                Rect rect = new Rect(position.x, positionY, position.width, EditorGUI.GetPropertyHeight(property));
                positionY += rect.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, property, true);
            } while(property.NextVisible(false) && property.depth == depth);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.hasVisibleChildren == false) return 0;
            property.NextVisible(true);
            int depth = property.depth;
            float height = 0;
            do {
                height += EditorGUI.GetPropertyHeight(property, true);
            } while(property.NextVisible(false) && property.depth == depth);
            return height;
        }
    }
}