using System;
using UnityEditor;
using UnityEngine;

namespace TKO.Core.PropertyAttributes
{
    [CustomPropertyDrawer(typeof(EnumAsIndexAttribute))]
    public class EnumAsIndexPropertyDrawer : ArrayElementPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!CheckIfArrayProperty(position, property, label)) 
                return;

            EnumAsIndexAttribute enumAsIndexAttribute = (EnumAsIndexAttribute) attribute;
            string[] names = Enum.GetNames(enumAsIndexAttribute.EnumType);

            int index = GetPropertyIndexByPath(property.propertyPath);
            
            string arrayName = GetPropertyArrayPathByItemPath(property.propertyPath);
            SerializedProperty arrayProperty = property.serializedObject.FindProperty(arrayName);
            
            if (arrayProperty.arraySize != names.Length)
            {
                arrayProperty.arraySize = names.Length;
            }
            
            EditorGUI.PropertyField(position, property, new GUIContent(names[index]));
        }
    }
}