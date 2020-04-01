using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.Framework.UI
{
    public static class VisualComponentFactory
    {
        private static readonly Type INTERFACE_TYPE = typeof(IVisualComponent);
        private static readonly BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void InitializeVisualComponent(IVisualComponent visualComponent)
        {
            VisualElement visualElement = visualComponent as VisualElement;
            if (visualElement == null)
            {
                throw new Exception("Can only initialize Visual Elements");
            }

            AddTypeClasses(visualElement, visualComponent);
            LoadStyleSheet(visualElement, visualComponent);
            if (LoadTemplate(visualElement, visualComponent))
                InitializeChildReferences(visualElement, visualComponent);
        }

        private static void AddTypeClasses(VisualElement visualElement, IVisualComponent visualComponent)
        {
            Type type = visualComponent.GetType();
            //Iterate over the type hierarchy till it reaches the visual component interface
            while (type != null && INTERFACE_TYPE.IsAssignableFrom(type))
            {
                visualElement.AddToClassList(type.Name);
                type = type.BaseType;
            }
        }

        private static void LoadStyleSheet(VisualElement visualElement, IVisualComponent visualComponent)
        {
            if (string.IsNullOrEmpty(visualComponent.StyleSheetPath))
                return;

            StyleSheet styleSheet = Resources.Load<StyleSheet>(visualComponent.StyleSheetPath);
            if (styleSheet == null)
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(visualComponent.StyleSheetPath);

            if (styleSheet == null)
            {
                Debug.LogError($"Stylesheet at path {visualComponent.StyleSheetPath} not found.");
                return;
            }

            visualElement.styleSheets.Add(styleSheet);
        }

        private static bool LoadTemplate(VisualElement visualElement, IVisualComponent visualComponent)
        {
            if (string.IsNullOrEmpty(visualComponent.TemplatePath))
                return false;

            VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>(visualComponent.TemplatePath);
            if (visualTreeAsset == null)
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualComponent.TemplatePath);

            if (visualTreeAsset == null)
            {
                Debug.LogError($"Template at path {visualComponent.TemplatePath} not found.");
                return false;
            }

            visualTreeAsset.CloneTree(visualElement);
            return true;
        }

        private static void InitializeChildReferences(VisualElement visualElement, IVisualComponent visualComponent)
        {
            Type type = visualComponent.GetType();
            //Iterate over the type hierarchy till it reaches the visual component interface
            while (type != null && INTERFACE_TYPE.IsAssignableFrom(type))
            {
                InitializeChildReferences(visualElement, visualComponent, type);
                type = type.BaseType;
            }
        }

        private static void InitializeChildReferences(VisualElement visualElement, IVisualComponent visualComponent, Type type)
        {
            FieldInfo[] fields = type.GetFields(BINDING_FLAGS);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                ViewChildAttribute attribute = field.GetCustomAttribute<ViewChildAttribute>();
                if (attribute == null)
                    continue;

                VisualElement childElement = visualElement.Q(attribute.Name, attribute.ClassName);
                if (childElement == null)
                    continue;

                if (childElement.GetType().IsAssignableFrom(field.FieldType))
                    field.SetValue(visualComponent, childElement);
            }
        }
    }
}
