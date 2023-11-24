using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCResourceManager : MonoBehaviour
    {
        // Singleton
        private static UCResourceManager _Singleton;
        public static UCResourceManager Singleton => _Singleton;

        [Header("Upgrade Icons")]
        public Sprite U_GoldIncome;
        public Sprite U_Damage;

        private Dictionary<UpgradeId, Sprite> UpgradeSprites;

        [Header("Prefabs")]
        public UI_Upgrade UpgradePrefab;

        public void Start()
        {
            _Singleton = GameObject.Find("ResourceManager").GetComponent<UCResourceManager>();

            UpgradeSprites = new Dictionary<UpgradeId, Sprite>()
            {
                {UpgradeId.GoldIncome, U_GoldIncome },
                {UpgradeId.Damage, U_Damage },
            };
        }

        public Sprite GetUpgradeSprite(Upgrade upgrade) => UpgradeSprites[upgrade.Id];

    }
}
