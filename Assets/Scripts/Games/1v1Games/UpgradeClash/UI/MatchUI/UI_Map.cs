using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public GameObject Mill;

        public void Init(Player player)
        {
            Player = player;
        }

        public void UpdateVales()
        {
            SetWorkerAmount(FoodWorkerContainer, Player.Units[UnitId.FoodWorker]);
            SetWorkerAmount(WoodWorkerContainer, Player.Units[UnitId.WoodWorker]);
            SetWorkerAmount(GoldWorkerContainer, Player.Units[UnitId.GoldWorker]);
            SetWorkerAmount(StoneWorkerContainer, Player.Units[UnitId.StoneWorker]);

            Palisade.SetActive(Player.HasBuilding(BuildingId.Palisade));
            Barracks.SetActive(Player.HasBuilding(BuildingId.Barracks));
            Mill.SetActive(Player.HasBuilding(BuildingId.Mill));
        }

        private void SetWorkerAmount(GameObject container, Unit unit)
        {
            HelperFunctions.DestroyAllChildredImmediately(container);
            for (int i = 0; i < unit.MaxAmount; i++)
            {
                Image unitImg = Instantiate(UCResourceManager.Singleton.UnitPrefab, container.transform);
                unitImg.color = i < unit.Amount ? Color.black : Color.white;
            }
        }
    }
}
