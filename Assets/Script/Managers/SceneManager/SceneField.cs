using UnityEngine;

[System.Serializable]
public class SceneFieldAsset
{
	[SerializeField] Object m_sceneAsset;
	[SerializeField] string m_sceneName = "";

	public string SceneName
	{
		get { return m_sceneName; }
	}
	
	public static implicit operator string(SceneFieldAsset sceneField)
	{
		return sceneField.SceneName;
	}
}