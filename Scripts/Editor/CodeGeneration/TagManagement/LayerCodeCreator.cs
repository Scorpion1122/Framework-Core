using System;
using UnityEngine;
using System.Text;
using UnityEditorInternal;
using System.IO;

namespace TKO.Framework.CodeGeneration.TagManagement
{
    public class LayerCodeCreator
    {
        private const string header =
            "/// <summary>\n" +
            "/// <author>Code Generator</author>\n" +
            "/// </summary>\n" +
            "public static class Layers\n" +
            "{\n";

        private const string item =
            "\tpublic static int {0} = {1};\n" +
            "\tpublic static int {0}_MASK = {0};\n";

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
            string[] layers = GetLayers();
            for (int i = 0; i < layers.Length; i++)
            {
                string formattedLayer = StringUtility.AllCapsFormatter(layers[i]);
                int value = GetLayerValue(layers[i]);

                builder.AppendFormat(item, formattedLayer, value.ToString());
            }
        }

        private int GetLayerValue(string name)
        {
            return LayerMask.NameToLayer(name);
        }

        private string[] GetLayers()
        {
            return InternalEditorUtility.layers;
        }

        private void AddFooter(StringBuilder builder)
        {
            builder.Append(footer);
        }

        private void SaveFile(string path, string content)
        {
            string filePath = string.Format("{0}/Layers.cs", path);
            if (Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(filePath, content);
        }
    }
}