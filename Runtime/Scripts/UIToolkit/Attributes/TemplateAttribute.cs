using System;

namespace TKO.UI.Toolkit
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TemplateAttribute : Attribute
	{
		public string Name { get; private set; }
        
		public TemplateAttribute(string name = null)
		{
			Name = name;
		}
	}
}
