using System;
using System.Text;
using System.IO;
using UnityEditorInternal;

namespace Thijs.Framework.CodeGeneration.TagManagement
{
    public class TagCodeCreator
    {
        private const string header =
            "/// <summary>\n" +
            "/// <author>Code Generator</author>\n" +
            "/// </summary>\n" +
            "public static class Tags\n" +
            "{\n";

        private const string item = "\tpublic const string {0} = \"{1}\";\n";

        private const string footer = "}";

        public void GeneratorCode(string path)
        {
            StringBuilder builder = new StringBuilder();
            AddHeader(builder);
            AddContent(builder);
            AddFooter(builder);

            SaveFile(path, builder.ToString());
        }

        private void AddHeader(StringBuilder builder)
        {
            builder.Append(header);
        }

        private void AddContent(StringBuilder builder)
        {
            string[] tags = InternalEditorUtility.tags;
            for (int i = 0; i < tags.Length; i++)
            {
                string formattedTag = StringUtility.AllCapsFormatter(tags[i]);
                builder.AppendFormat(item, formattedTag, tags[i]);
            }
        }

        private void AddFooter(StringBuilder builder)
        {
            builder.Append(footer);
        }

        private void SaveFile(string path, string content)
        {
            string filePath = string.Format("{0}/Tags.cs", path);
            if (Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(filePath, content);
        }
    }
}