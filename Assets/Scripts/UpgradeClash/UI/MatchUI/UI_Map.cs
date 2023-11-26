using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UI_Map : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("Elements")]
        public GameObject FoodWorkerContainer;
        public GameObject WoodWorkerContainer;
        public GameObject GoldWorkerContainer;
        public GameObject StoneWorkerContainer;

        public GameObject Palisade;
        public GameObject Barracks;

        public void Init(Player player)
        {
            Player = player;
        }

        public void UpdateVales()
        {
            SetWorkerAmount(FoodWorkerContainer, Player.Units[UnitId.FoodWorker].Amount);
            SetWorkerAmount(WoodWorkerContainer, Player.Units[UnitId.WoodWorker].Amount);
            SetWorkerAmount(GoldWorkerContainer, Player.Units[UnitId.GoldWorker].Amount);
            SetWorkerAmount(StoneWorkerContainer, Player.Units[UnitId.StoneWorker].Amount);

            Palisade.SetActive(Player.BuildingUpgrades[BuildingId.Palisade].IsInEffect);
            Barracks.SetActive(Player.BuildingUpgrades[BuildingId.Barracks].IsInEffect);
        }

        private void SetWorkerAmount(GameObject container, int numWorkers)
        {
            HelperFunctions.DestroyAllChildredImmediately(container);
            for (int i = 0; i < numWorkers; i++) Instantiate(UCResourceManager.Singleton.UnitPrefab, container.transform);
        }
    }
}
