using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    public void ButtonBack()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
    
    public void ButtonRestart(string targetScene)
    {
        Game_Manager game_Manager = FindObjectOfType<Game_Manager>();
        if(game_Manager != null)
            game_Manager.ResetEntries();
        ButtonBack();
        SceneManager.LoadScene(targetScene);
    }
    
    public void ButtonQuit()
    {
        Application.Quit();
    }
}
