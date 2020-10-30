using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.UI.Toolkit
{
    public static class VisualElementExtentions
    {
        private static readonly BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void InitializeReferences(this VisualElement visualElement)
        {
            LoadTemplate(visualElement);
            LoadStyleSheet(visualElement);
            InitializeChildReferences(visualElement);
        }

        private static void LoadStyleSheet(VisualElement visualElement)
        {
            Type type = visualElement.GetType();
            StyleSheetAttribute styleSheetAttribute = type.GetCustomAttribute<StyleSheetAttribute>();

            if(styleSheetAttribute == null)
            {
                return;
            }

            if(styleSheetAttribute.Names == null)
            {
                LoadStyleSheet(type.Name.ToLower(), visualElement);
            }
            else
            {
                for(int i = 0, length = styleSheetAttribute.Names.Length; i < length; i++)
                {
                    LoadStyleSheet(styleSheetAttribute.Names[i], visualElement);
                }
            }
        }

        private static void LoadStyleSheet(string styleSheetName, VisualElement visualElement)
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(styleSheetName);
            
#if UNITY_EDITOR
            if(styleSheet == null)
            {
                string[] assets = UnityEditor.AssetDatabase.FindAssets($"t:StyleSheet {styleSheetName}");
                if(assets.Length == 1)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
                    styleSheet = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                }
                else if(assets.Length > 1)
                {
                    Debug.LogError($"Multiple stylesheets found with the name {styleSheetName}");
                }
            }
#endif
        
            if (styleSheet == null)
            {
                Debug.LogError($"Stylesheet {styleSheetName} not found.");
                return;
            }
        
            visualElement.styleSheets.Add(styleSheet);
        }
        
        private static void LoadTemplate(VisualElement visualElement)
        {
            Type type = visualElement.GetType();
            TemplateAttribute templateAttribute = type.GetCustomAttribute<TemplateAttribute>();

            if(templateAttribute == null)
            {
                return;
            }

            string templateName = templateAttribute.Name ?? type.Name.ToLower();
            VisualTreeAsset templateAsset = Resources.Load<VisualTreeAsset>(templateName);
            
#if UNITY_EDITOR
            if(templateAsset == null)
            {
                string[] assets = UnityEditor.AssetDatabase.FindAssets($"t:VisualTreeAsset {templateName}");
                if(assets.Length == 1)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
                    templateAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                }
                else if(assets.Length > 1)
                {
                    Debug.LogError($"Multiple templates found with the name {templateName}");
                }
            }
#endif
        
            if (templateAsset == null)
            {
                Debug.LogError($"Template {templateName} not found.");
                return;
            }
        
            templateAsset.CloneTree(visualElement);
        }

        private static void InitializeChildReferences(VisualElement visualElement)
        {
            Type type = visualElement.GetType();
            //Iterate over the type hierarchy till it reaches the visual component interface
            while (type != null && typeof(VisualElement).IsAssignableFrom(type))
            {
                InitializeChildReferences(visualElement, type);
                type = type.BaseType;
            }
        }

        private static void InitializeChildReferences(VisualElement visualElement, Type type)
        {
            FieldInfo[] fields = type.GetFields(BINDING_FLAGS);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                ChildViewAttribute attribute = field.GetCustomAttribute<ChildViewAttribute>();
                if (attribute == null)
                    continue;

                VisualElement childElement = visualElement.Q(attribute.Name, attribute.ClassName);
                if(childElement == null)
                {
                    if(!attribute.Optional)
                    {
                        Debug.LogError($"Visual Element {visualElement.name} does not have a child element with the name {attribute.Name} and/or class name {attribute.ClassName}");
                    }
                    continue;
                }

                if(field.FieldType.IsInstanceOfType(childElement))
                {
                    field.SetValue(visualElement, childElement);
                }
                else
                {
                    if(!attribute.Optional)
                    {
                        Debug.LogError($"Visual Element {visualElement.name} does not have a type that is assignable to {field.FieldType}");
                    }
                }
            }
        }
    }
}