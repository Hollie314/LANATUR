using UnityEngine;

public class KillZone : MonoBehaviour
{
    private Game_Manager game_Manager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game_Manager = FindFirstObjectByType<Game_Manager>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("kill trigger entered");
        if (other.tag == "Player")
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.GetComponent<CharacterController>().transform.position = game_Manager.LastPositionSaved;
            other.GetComponent<CharacterController>().enabled = true;
            // other.transform.position = game_Manager.LastPositionSaved;
            Debug.Log("position changed");
            Debug.Log(other.name);
            Debug.Log(other.transform.position);
        }
    }
}
