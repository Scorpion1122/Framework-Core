using System;

namespace TKO.UI.Toolkit
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ChildViewAttribute : Attribute
	{
		public string Name { get; private set; }
		public string ClassName { get; private set; }
		public bool Optional { get; private set; }
        
		public ChildViewAttribute(string name = null, string className = null, bool optional = false)
		{
			Name = name;
			ClassName = className;
			Optional = optional;
		}
	}
}
