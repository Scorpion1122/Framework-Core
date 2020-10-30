using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TKO.UI.Toolkit
{
	public static partial class VisualElementExtentions
	{
		public static VisualElement CreateDefaultInspector(SerializedObject serializedObject)
		{
			var container = new VisualElement();
 
			var iterator = serializedObject.GetIterator();
			if (iterator.NextVisible(true))
			{
				do
				{
					var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
 
					if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
						propertyField.SetEnabled(value: false);
 
					container.Add(propertyField);
				}
				while (iterator.NextVisible(false));
			}
 
			return container;
		}
	}
}
