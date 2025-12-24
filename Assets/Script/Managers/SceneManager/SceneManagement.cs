using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class SceneManagement : MonoBehaviour
{
	public Action SceneLoadCallback;
	public static SceneManagement singleton = null;
	private Scene activeScene;
	private List<SceneFieldAsset> currentScenesToUnload;
	private int countCurrentLoadedScenes;
	private int countScenesToLoad;
	[Header("SET LAST LOADED SCENE AS ACTIVE")]
	[Tooltip("Should last loaded scene be active?")]
	[SerializeField] bool lastLoadedSceneIsActive;

	[Header("OBJECTS TO MOVE TO ACTIVE SCENE")]
	[Tooltip("These objects will be moved to the current active scene")]
	[SerializeField] GameObject[] objectsToMoveToActiveScene;
	List<string> tempLoadedScenes = new List<string>();

	void OnEnable()
	{
		SceneManager.sceneLoaded += SceneLoaded;
		SceneManager.activeSceneChanged += ActiveSceneChanged;
		SceneManager.sceneUnloaded += SceneUnloaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= SceneLoaded;
		SceneManager.activeSceneChanged -= ActiveSceneChanged;
		SceneManager.sceneUnloaded -= SceneUnloaded;
	}

	private void Awake()
	{
		if(singleton == null)
		{
			singleton = this;
			DontDestroyOnLoad(gameObject);
		}

		else if(singleton != this)
		{
			DestroyImmediate(gameObject);
		}
	}

	void SetLastLoadedSceneActiveScene()
	{
		SceneManager.SetActiveScene(activeScene);
	}

	public void SetActiveSceneManually(Scene activeScene)
	{
		lastLoadedSceneIsActive = false;
		SceneManager.SetActiveScene(activeScene);
		this.activeScene = activeScene;
	}

	void UnloadScenes()
	{
		countCurrentLoadedScenes = 0;

		foreach (var scene in currentScenesToUnload)
		{
			var alreadyLoaded = SceneManager.GetSceneByName(scene.SceneName).isLoaded;

			if (alreadyLoaded)
				SceneManager.UnloadSceneAsync(scene);
		}

		print(currentScenesToUnload.Count);
	}

	void ClearTempScenes()
	{
		if (tempLoadedScenes.Count > 0)
			tempLoadedScenes.Clear();
	}

	public void LoadScenes(SceneFieldAsset[] scenesToLoad)
	{
		countScenesToLoad = scenesToLoad.Length;

		foreach (var scene in scenesToLoad)
		{
			var alreadyLoaded = SceneManager.GetSceneByName(scene.SceneName).isLoaded;

			if (!alreadyLoaded)
				StartCoroutine(LoadSceneAdditive(scene));
		}
	}

	void MoveObjectsToActiveScene()
	{
		foreach (var go in objectsToMoveToActiveScene)
			SceneManager.MoveGameObjectToScene(go, activeScene);
	}

	IEnumerator LoadSceneAdditive(SceneFieldAsset scene)
	{
		var asyncOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

		while (!asyncOp.isDone)
		{
			Debug.Log("Loading scene: " + (asyncOp.progress * 100).ToString() + " %");
			yield return null;
		}

		if (asyncOp.isDone)
		{
			countCurrentLoadedScenes++;
			tempLoadedScenes.Add(scene.SceneName);
		}
		print(countCurrentLoadedScenes);
		if (countCurrentLoadedScenes >= countScenesToLoad)
		{
			SceneLoadCallback();
			MoveObjectsToActiveScene();
			UnloadScenes();
		}
	}

	public void SetScenesToUnload(SceneFieldAsset[] scenesToUnload)
	{
		currentScenesToUnload = new List<SceneFieldAsset>();

		foreach (var scene in scenesToUnload)
		{
			currentScenesToUnload.Add(scene);
		}
	}

	void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Scene loaded sucessufully: " + scene.name);

		if (lastLoadedSceneIsActive)
		{
			activeScene = scene;
			SetLastLoadedSceneActiveScene();
		}
	}

	void ActiveSceneChanged(Scene scene, Scene mode)
	{
		Debug.Log("Active scene changed. Previour scene: " + scene.name);
	}

	void SceneUnloaded(Scene scene)
	{
		Debug.Log("Scene " + scene.name + " unloaded");
	}
}
