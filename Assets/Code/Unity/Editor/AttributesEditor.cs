using UnityEditor;

using UnityEngine;

namespace SaloonSlingers.Unity.Editor
{
    [CustomEditor(typeof(Attributes))]
    public class AttributesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach ((var key, var attribute) in ((Attributes)target).Registry)
            {
                EditorGUILayout.LabelField(key.ToString());
                EditorGUILayout.LabelField("Max Value", attribute.MaxValue.ToString());
                EditorGUILayout.LabelField("Initial Value", attribute.InitialValue.ToString());
                EditorGUILayout.LabelField("Current Value", attribute.Value.ToString());
                EditorGUILayout.Space();
            }

            if (GUI.changed) EditorUtility.SetDirty(target);

        }
    }
}
