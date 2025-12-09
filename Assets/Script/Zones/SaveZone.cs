using UnityEngine;

public class SaveZone : MonoBehaviour
{
    private Game_Manager game_Manager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game_Manager = FindFirstObjectByType<Game_Manager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("save trigger entered");
        if (other.tag == "Player")
        {
            game_Manager.LastPositionSaved = this.transform.position;
            Debug.Log("position saved");
            Debug.Log(other.name);
            Debug.Log(this.transform.position);
        }
    }
}
