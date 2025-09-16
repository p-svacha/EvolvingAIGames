using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Incrementum
{
    public enum UpgradeUIState { Unacquired, Staged, Acquired }

    public class UI_IncrementumUpgrade : MonoBehaviour
    {
        private UI_PlayIncrementum Game;
        public UpgradeDef Def { get; private set; }
        public UpgradeUIState State { get; private set; } = UpgradeUIState.Unacquired;

        [Header("Colors")]
        public Color UnacquiredColor = new Color(0.25f, 0.25f, 0.25f);
        public Color StagedColor = new Color(0.35f, 0.35f, 0.6f);
        public Color AcquiredColor = new Color(0.25f, 0.6f, 0.25f);

        [Header("Elements")]
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI CostText;
        public TextMeshProUGUI EffectText;

        public Button Button;
        public Image Background;

        public void Init(UI_PlayIncrementum game, UpgradeDef def)
        {
            Game = game;
            Def = def;

            NameText.text = def.Label;
            CostText.text = def.GetCostString();
            EffectText.text = def.GetEffectString();

            Button.onClick.AddListener(OnClick);
            SetState(UpgradeUIState.Unacquired);
        }

        public void SetState(UpgradeUIState s)
        {
            State = s;
            switch (s)
            {
                case UpgradeUIState.Unacquired: Background.color = UnacquiredColor; break;
                case UpgradeUIState.Staged: Background.color = StagedColor; break;
                case UpgradeUIState.Acquired: Background.color = AcquiredColor; break;
            }
        }

        private void OnClick()
        {
            Game.OnUpgradeClicked(this);
        }
    }
}
