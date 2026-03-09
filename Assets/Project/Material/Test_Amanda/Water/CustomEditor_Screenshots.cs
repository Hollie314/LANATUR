#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Screenshots))]
class CustomEditor_Screenshots : Editor
{
    private Screenshots screenshots;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Take Screenshot"))
        {
            ScreenCapture.CaptureScreenshot("portfolio.png",1);
        }
    }
}
#endif