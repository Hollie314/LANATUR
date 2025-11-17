using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SwitchToggle : MonoBehaviour
{
    float float_fillSpeed = 5f;

    // References to UI elements
    public Image image_Fill;
    public Image image_Toggle;
    public TextMeshProUGUI text_State;

    // Variables
    private RectTransform transform_Toggle;
    private bool bool_isON = false;
    private float float_targetFillAmount;
    public float posX_On;
    public float posX_Off;

    // Colors for text in different states
    public Color color_On = new Color(37f / 255f, 37f / 255f, 37f / 255f);
    public Color color_Off = Color.white;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        transform_Toggle = image_Toggle.GetComponent<RectTransform>();

        //transform_Toggle.pivot = new Vector2(0f, 0.5f);

        float_targetFillAmount = bool_isON ? 1f : 0f;

        UpdateTogglePivot();
        UpdateFillAmount();
        UpdateStateText();
    }

    public void Toggle()
    {
        bool_isON = !bool_isON;

        float_targetFillAmount = bool_isON ? 1f : 0f;

        UpdateTogglePivot();
        UpdateStateText();
    }

    // Update is called once per frame
    void Update()
    {
        image_Fill.fillAmount = Mathf.Lerp(image_Fill.fillAmount, float_targetFillAmount, Time.deltaTime * float_fillSpeed);

        float PosX = bool_isON ? posX_On : posX_Off;
        transform_Toggle.position = new Vector2(Mathf.Lerp(transform_Toggle.position.x, PosX, Time.deltaTime * float_fillSpeed * 1.3f), transform_Toggle.position.y);
        // transform_Toggle.anchoredPosition = new Vector2(Mathf.Lerp(transform_Toggle.anchoredPosition.x, PosX, Time.deltaTime * 8f), transform_Toggle.anchoredPosition.y);
    }

    private void UpdateFillAmount()
    {
        image_Fill.fillAmount = float_targetFillAmount;

        //transform_Toggle.anchoredPosition = new Vector2(float_targetFillAmount * image_Fill.rectTransform.rect.width, transform_Toggle.anchoredPosition.y);
    }

    private void UpdateStateText()
    {
        text_State.text = bool_isON ? "ON" : "OFF";
        text_State.color = bool_isON ? color_On : color_Off;
    }

    private void UpdateTogglePivot()
    {
        if (bool_isON)
        {
            //transform_Toggle.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            //transform_Toggle.pivot = new Vector2(0f, 0.5f);
        }
    }
}