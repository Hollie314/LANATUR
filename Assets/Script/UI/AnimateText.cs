using TMPro;
using UnityEngine;

public class AnimateText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] texts;
    [SerializeField] private float timeToSwitch;
    [SerializeField] private bool loop;

    private int index = 0;
    private float currentTime = 0f;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > timeToSwitch)
        {
            text.text = texts[index];
            index = (index + 1) % texts.Length;
            currentTime = 0f;
        }
    }
}
