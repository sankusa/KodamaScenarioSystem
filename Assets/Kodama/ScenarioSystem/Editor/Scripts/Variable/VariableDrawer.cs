using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;
using System;
using System.Linq;
using UnityEditorInternal;

namespace Kodama.ScenarioSystem.Editor {
    [CustomPropertyDrawer(typeof(Variable<>), true)]
    public class VariableDrawer : PropertyDrawer {
        private static RectUtil.LayoutLength[] rectLengths = new RectUtil.LayoutLength[]{ new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1), new RectUtil.LayoutLength(1.5f)};
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            List<Rect> rects = RectUtil.DivideRectHorizontal(rect, rectLengths);
            rects[0] = new Rect(rects[0].x, rects[0].y, rects[0].width, EditorGUIUtility.singleLineHeight);
            rects[1] = new Rect(rects[1].x, rects[1].y, rects[1].width - 4, EditorGUIUtility.singleLineHeight);
            

            string[] splitedPaths = property.propertyPath.Split('[', ']');
            string typeName = ((Scenario)property.serializedObject.targetObject)
                .Variables[int.Parse(splitedPaths[1])]
                .GetType()
                .BaseType
                .GetField("_value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .FieldType
                .Name;

            typeName = TypeNameUtil.ConvertToPrimitiveTypeName(typeName);

            EditorGUI.LabelField(rects[0], typeName);

            EditorGUI.PropertyField(rects[1], property.FindPropertyRelative("_name"), GUIContent.none);

            SerializedProperty valueProp = property.FindPropertyRelative("_value");
            if(valueProp != null) {
                EditorGUI.PropertyField(rects[2], valueProp, GUIContent.none, true);
            }
            else {
                EditorGUI.LabelField(rects[2], "null");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty valueProp = property.FindPropertyRelative("_value");
            if(valueProp != null) {
                if(valueProp.propertyType == SerializedPropertyType.Vector2
                    || valueProp.propertyType == SerializedPropertyType.Vector3
                    || valueProp.propertyType == SerializedPropertyType.Vector4
                    || valueProp.propertyType == SerializedPropertyType.Rect
                    || valueProp.propertyType == SerializedPropertyType.Bounds) {
                        return EditorGUI.GetPropertyHeight(valueProp, true) - EditorGUIUtility.singleLineHeight - 2;
                }
                else {
                    return EditorGUI.GetPropertyHeight(valueProp, true);
                }
            }
            else {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}