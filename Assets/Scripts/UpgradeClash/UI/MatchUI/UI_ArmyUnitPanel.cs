using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UpgradeClash
{
    public class UI_ArmyUnitPanel : MonoBehaviour
    {
        public ArmyUnit Army { get; private set; }

        [Header("Elements")]
        public Image Icon;

        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI CooldownText;

        public GameObject ProgressBarContainer;
        public GameObject ProgressDynamicBar;
        public TextMeshProUGUI ProgressText;

        public GameObject UnitContainer1;
        public GameObject UnitContainer2;

        public void Init(ArmyUnit army)
        {
            Army = army;
        }

        public void UpdateValues()
        {
            gameObject.SetActive(Army.Amount > 0);

            Icon.sprite = UCResourceManager.Singleton.GetUnitSprite(Army.Id);

            DamageText.text = Army.GetAttackDamage().ToString();
            CooldownText.text = Army.GetCooldown().ToString();

            float fullWidth = ProgressBarContainer.GetComponent<RectTransform>().rect.width;
            float dynamicBarWidth = (1f - ((float)Army.RemainingCooldown / Army.GetCooldown())) * fullWidth;
            ProgressDynamicBar.GetComponent<RectTransform>().sizeDelta = new Vector2(dynamicBarWidth, ProgressDynamicBar.GetComponent<RectTransform>().sizeDelta.y);
            ProgressText.text = System.TimeSpan.FromSeconds(Army.RemainingCooldown).ToString(@"m\:ss");

            HelperFunctions.DestroyAllChildredImmediately(UnitContainer1);
            HelperFunctions.DestroyAllChildredImmediately(UnitContainer2);
            for(int i = 0; i < Army.Amount; i++)
            {
                if (i < 5) Instantiate(UCResourceManager.Singleton.UnitPrefab, UnitContainer1.transform);
                else Instantiate(UCResourceManager.Singleton.UnitPrefab, UnitContainer2.transform);
            }
        }
    }
}
