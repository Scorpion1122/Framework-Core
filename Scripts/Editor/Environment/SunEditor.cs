using UnityEditor;

namespace TKO.Core.Environment
{
    [CustomEditor(typeof(Sun))]
    public class SunEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Sun sun = (Sun) target;
                sun.InitializeAndUpdatePosition();
            }
        }
    }
}