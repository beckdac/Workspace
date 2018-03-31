using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateBaracade))]
public class GenerateBaracadeEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateBaracade generateBaracadeScript = (GenerateBaracade)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Baracade"))
        {
            generateBaracadeScript.generateBaracade();
        }
        if (GUILayout.Button("Reset"))
        {
            generateBaracadeScript.resetBaracade();
        }
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }
}
