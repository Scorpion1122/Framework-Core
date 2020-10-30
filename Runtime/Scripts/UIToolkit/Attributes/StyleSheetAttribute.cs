using System;

namespace TKO.UI.Toolkit
{
	[AttributeUsage(AttributeTargets.Class)]
	public class StyleSheetAttribute : Attribute
	{
		public string[] Names { get; private set; }
        
		public StyleSheetAttribute(params string[] names)
		{
			Names = names;
		}
	}
}
