using UnityEngine;
using UnityEditor;

namespace Thijs.Framework.CodeGeneration.TagManagement
{
    public class TagManagerAssetListener : AssetPostprocessor
    {

        private const string TAG_MANAGER_KEY = "TagManager.asset";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i].Contains(TAG_MANAGER_KEY))
                {
                    GenerateLayers();
                    GenerateTags();
                    return;
                }
            }
        }

        [MenuItem("CodeGeneration/Generate Layers")]
        public static void GenerateLayers()
        {
            LayerCodeCreator generator = new LayerCodeCreator();
            generator.GeneratorCode(CodeGenSettings.GENERATED_CODE_FOLDER);

            AssetDatabase.Refresh();
        }

        [MenuItem("CodeGeneration/Generate Tags")]
        public static void GenerateTags()
        {
            TagCodeCreator generator = new TagCodeCreator();
            generator.GeneratorCode(CodeGenSettings.GENERATED_CODE_FOLDER);

            AssetDatabase.Refresh();
        }
    }
}