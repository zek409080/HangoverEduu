using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameResetManager))]
public class GameResetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameResetManager gameResetManager = (GameResetManager)target;
        if (GUILayout.Button("Reset All Data"))
        {
            gameResetManager.ResetAllData();
        }
    }
}