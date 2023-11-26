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
        public Sprite FoodWorkerIcon;
        public Sprite WoodWorkerIcon;
        public Sprite GoldWorkerIcon;
        public Sprite StoneWorkerIcon;
        public Sprite MilitiaIcon;
        public Sprite U_Damage;

        private Dictionary<UpgradeId, Sprite> UpgradeSprites;
        private Dictionary<UnitId, Sprite> UnitSprites;

        [Header("Prefabs")]
        public UI_Upgrade UpgradePrefab;
        public GameObject UnitPrefab;

        public void Start()
        {
            _Singleton = GameObject.Find("ResourceManager").GetComponent<UCResourceManager>();

            UpgradeSprites = new Dictionary<UpgradeId, Sprite>()
            {
                {UpgradeId.TrainFoodWorker, FoodWorkerIcon },
                {UpgradeId.TrainWoodWorker, WoodWorkerIcon },
                {UpgradeId.TrainGoldWorker, GoldWorkerIcon },
                {UpgradeId.TrainStoneWorker, StoneWorkerIcon },
                {UpgradeId.TrainMilitia, MilitiaIcon },
                {UpgradeId.Damage, U_Damage },
            };

            UnitSprites = new Dictionary<UnitId, Sprite>()
            {
                {UnitId.FoodWorker, FoodWorkerIcon },
                {UnitId.WoodWorker, WoodWorkerIcon },
                {UnitId.GoldWorker, GoldWorkerIcon },
                {UnitId.StoneWorker, StoneWorkerIcon },
                {UnitId.Militia, MilitiaIcon },
            };
        }

        public Sprite GetUpgradeSprite(UpgradeId id) => UpgradeSprites[id];
        public Sprite GetUnitSprite(UnitId id) => UnitSprites[id];

    }
}
