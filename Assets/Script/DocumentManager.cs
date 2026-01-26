using System.Collections.Generic;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance;

    private Dictionary<string, string> collectedLetters = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddLetter(string id, string content)
    {
        if (!collectedLetters.ContainsKey(id))
        {
            collectedLetters.Add(id, content);
            Debug.Log("Lettre ajoutée à la collection");
        }
    }

    public string GetLetter(string id)
    {
        return collectedLetters.ContainsKey(id) ? collectedLetters[id] : null;
    }
}