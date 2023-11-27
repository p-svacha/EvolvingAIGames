using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_ProgressBar : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;
    public GameObject ProgressBar;
    public TextMeshProUGUI ProgressText;

    public void UpdateValues(float value, float maxValue, string text = "", bool reverse = false)
    {
        float fullWidth = Container.GetComponent<RectTransform>().rect.width;
        float ratio = value / maxValue;
        if (reverse) ratio = 1f - ratio;
        float dynamicBarWidth = ratio * fullWidth;
        ProgressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(dynamicBarWidth, ProgressBar.GetComponent<RectTransform>().sizeDelta.y);
        if(ProgressText != null) ProgressText.text = text;
    }

    public void SetBarColor(Color color)
    {
        ProgressBar.GetComponent<Image>().color = color;
    }
}
