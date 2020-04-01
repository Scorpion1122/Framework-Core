using System;
using UnityEngine;

namespace TKO.Core.PropertyAttributes
{
    public class EnumAsIndexAttribute : PropertyAttribute
    {
        public Type EnumType { get; private set; }
        
        public EnumAsIndexAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("This attribute only works with enums!");
            EnumType = enumType;
        }
    }
}