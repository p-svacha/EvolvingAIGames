using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeClash
{
    public class UI_SpeedControl : MonoBehaviour
    {
        [Header("Elements")]
        public Button PauseButton; 
        public Button Speed1Button; 
        public Button Speed2Button; 
        public Button Speed3Button;

        public void Init(UCMatchUI UI)
        {
            PauseButton.onClick.AddListener(() => UI.SetSpeed0());
            Speed1Button.onClick.AddListener(() => UI.SetSpeed1());
            Speed2Button.onClick.AddListener(() => UI.SetSpeed2());
            Speed3Button.onClick.AddListener(() => UI.SetSpeed3());
        }

        public void HighlightButton(Button button)
        {
            PauseButton.GetComponent<Image>().color = (PauseButton == button) ? Color.yellow : Color.white;
            Speed1Button.GetComponent<Image>().color = (Speed1Button == button) ? Color.yellow : Color.white;
            Speed2Button.GetComponent<Image>().color = (Speed2Button == button) ? Color.yellow : Color.white;
            Speed3Button.GetComponent<Image>().color = (Speed3Button == button) ? Color.yellow : Color.white;
        }
    }
}
