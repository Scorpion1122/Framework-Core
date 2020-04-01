using System.Collections.Generic;
using System.Reflection;

namespace TKO.Core.Services
{
    public class InjectableDefinition
    {
        private readonly Dictionary<FieldInfo, InjectAttribute> fields;
        private readonly Dictionary<PropertyInfo, InjectAttribute> properties;

        public Dictionary<FieldInfo, InjectAttribute> Fields { get { return fields; } }
        public Dictionary<PropertyInfo, InjectAttribute> Properties { get { return properties; } }

        public InjectableDefinition()
        {
            fields = new Dictionary<FieldInfo, InjectAttribute>();
            properties = new Dictionary<PropertyInfo, InjectAttribute>();
        }

        public void Add(MemberInfo info, InjectAttribute attribute)
        {
            FieldInfo field = info as FieldInfo;
            if (field != null)
            {
                AddField(field, attribute);
                return;
            }

            PropertyInfo property = info as PropertyInfo;
            if (property != null)
                AddProperty(property, attribute);
        }

        private void AddField(FieldInfo info, InjectAttribute attribute)
        {
            fields[info] = attribute;
        }

        private void AddProperty(PropertyInfo info, InjectAttribute attribute)
        {
            properties[info] = attribute;
        }
    }
}
