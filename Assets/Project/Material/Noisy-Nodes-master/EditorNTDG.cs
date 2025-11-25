using System.Xml.Schema;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(NoiseT3DGeneratorScript))]
//aaaa
class EditorNTDG : Editor
{
    
    public override void OnInspectorGUI()
    {
        NoiseT3DGeneratorScript noiseGenerator = (NoiseT3DGeneratorScript)target;
        base.OnInspectorGUI();

        // int selected = 0;
        // string[] options = new string[]
        // {
        //     "Perlin Noise", "Simplex Noise", "White Noise"
        // };
        //
        // selected = EditorGUILayout.Popup("Noise Type",selected, options);
        //

        if (GUILayout.Button("Generate Noise Texture"))
        {
            Debug.Log("Click !");
            noiseGenerator.buttonClick(noiseGenerator.imageSize, noiseGenerator.noiseType);
            Debug.Log("Done !");
        }
    }
}
