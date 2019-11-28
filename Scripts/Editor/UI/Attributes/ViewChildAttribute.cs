using System;

namespace Thijs.Framework.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ViewChildAttribute : Attribute
    {
        public string Name { get; private set; }
        public string ClassName { get; private set; }
        
        public ViewChildAttribute(string name = null, string className = null)
        {
            Name = name;
            ClassName = className;
        }
    }
}