using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneFieldAsset))]
public class SceneFieldDrawer : PropertyDrawer
{
	public override void OnGUI(Rect rectPosition, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(rectPosition, GUIContent.none, property);
		SerializedProperty sceneAsset = property.FindPropertyRelative("m_sceneAsset");
		SerializedProperty sceneName = property.FindPropertyRelative("m_sceneName");
		rectPosition = EditorGUI.PrefixLabel(rectPosition, GUIUtility.GetControlID(FocusType.Passive), label);

		if (sceneAsset != null)
		{
			sceneAsset.objectReferenceValue = EditorGUI.ObjectField(rectPosition, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

			if (sceneAsset.objectReferenceValue != null)
			{
				sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
			}
		}

		EditorGUI.EndProperty();
	}
}