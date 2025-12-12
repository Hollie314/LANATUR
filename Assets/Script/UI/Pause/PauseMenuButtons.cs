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
    
    public void ButtonRestart()
    {
        Game_Manager game_Manager = FindObjectOfType<Game_Manager>();
        game_Manager.ResetEntries();
        ButtonBack();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ButtonQuit()
    {
        Application.Quit();
    }
}
