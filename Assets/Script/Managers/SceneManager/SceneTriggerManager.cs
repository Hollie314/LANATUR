using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class SceneTriggerManager : MonoBehaviour 
{
	[Header("SCENES TO LOAD")]
	[SerializeField] SceneFieldAsset[] m_scenesToLoad;

	[Header("SCENES TO UNLOAD")]
	[SerializeField] SceneFieldAsset[] m_scenesToUnload;

	[Header("SCENES SETUP")]
	[SerializeField] int sceneIndexToBeActive;
	[SerializeField] bool activeSceneManually;

	[Header("PREVENT EXCESSIVE LOADINGS")]
	[SerializeField] float timeToActivateCollider;
	private BoxCollider thisBoxCollider;

	private void Start()
	{
		AssignBoxCollider();
		StartCoroutine(ActivateCollider());

		if (activeSceneManually)
		{
			try
			{
				print(SceneManager.GetSceneByName(m_scenesToLoad[sceneIndexToBeActive].SceneName));
			}

			catch(Exception e)
			{
				print(e);
				Debug.LogError("Please, select an index from: " + 0 + " to " + (m_scenesToLoad.Length - 1) + " on scenesToLoad array");
			}
		}
	}

	IEnumerator ActivateCollider()
	{
		yield return new WaitForSeconds(timeToActivateCollider);

		if (thisBoxCollider)
			thisBoxCollider.enabled = true;
	}

	void AssignBoxCollider()
	{
		thisBoxCollider = GetComponent<BoxCollider>();

		if (thisBoxCollider)
			thisBoxCollider.enabled = false;
	}


	void ManageScenes()
	{
		if (activeSceneManually)
			SceneManagement.singleton.SceneLoadCallback = SetActiveSceneManually;

		SceneManagement.singleton.LoadScenes(m_scenesToLoad);
		SceneManagement.singleton.SetScenesToUnload(m_scenesToUnload);
	}

	void SetActiveSceneManually()
	{
		var sceneExists = m_scenesToLoad[sceneIndexToBeActive] != null;

		if (sceneExists)
			SceneManagement.singleton.SetActiveSceneManually(SceneManager.GetSceneByName(m_scenesToLoad[sceneIndexToBeActive].SceneName));
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			ManageScenes();
		}
	}
}
