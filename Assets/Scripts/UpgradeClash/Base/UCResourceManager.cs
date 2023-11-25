using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeClash
{
    public class UCResourceManager : MonoBehaviour
    {
        // Singleton
        private static UCResourceManager _Singleton;
        public static UCResourceManager Singleton => _Singleton;

        [Header("Upgrade Icons")]
        public Sprite U_FoodWorker;
        public Sprite U_WoodWorker;
        public Sprite U_GoldWorker;
        public Sprite U_StoneWorker;
        public Sprite MilitiaIcon;
        public Sprite U_Damage;

        private Dictionary<UpgradeId, Sprite> UpgradeSprites;

        [Header("Prefabs")]
        public UI_Upgrade UpgradePrefab;
        public GameObject WorkerPrefab;
        public Image ArmyPrefab;

        public void Start()
        {
            _Singleton = GameObject.Find("ResourceManager").GetComponent<UCResourceManager>();

            UpgradeSprites = new Dictionary<UpgradeId, Sprite>()
            {
                {UpgradeId.TrainFoodWorker, U_FoodWorker },
                {UpgradeId.TrainWoodWorker, U_WoodWorker },
                {UpgradeId.TrainGoldWorker, U_GoldWorker },
                {UpgradeId.TrainStoneWorker, U_StoneWorker },
                {UpgradeId.TrainMilitia, MilitiaIcon },
                {UpgradeId.Damage, U_Damage },
            };
        }

        public Sprite GetUpgradeSprite(Upgrade upgrade) => UpgradeSprites[upgrade.Id];

    }
}
