using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlinkoBoard))]
public class PlinkoBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);

        if (GUILayout.Button("Generate Board", GUILayout.Height(28)))
        {
            var board = (PlinkoBoard)target;
            Undo.RecordObject(board.gameObject, "Generate Plinko Board");
            board.GenerateBoard();
        }
    }
}
