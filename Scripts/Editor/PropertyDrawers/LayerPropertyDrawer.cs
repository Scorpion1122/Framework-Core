using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Thijs.Core.PropertyAttributes
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] layers = InternalEditorUtility.layers;
            string selectedName = LayerMask.LayerToName(property.intValue);
            int selectedIndex = Array.IndexOf(layers, selectedName);
            
            int newSelectedIndex = EditorGUI.Popup(position, label.ToString(), selectedIndex, layers);

            if (newSelectedIndex != selectedIndex)
                property.intValue = LayerMask.NameToLayer(layers[newSelectedIndex]);
        }
    }
}
