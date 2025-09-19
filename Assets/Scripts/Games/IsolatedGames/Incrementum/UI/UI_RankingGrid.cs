using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Incrementum
{
    public class UI_RankingGrid : MonoBehaviour
    {
        private List<UI_RankingSquare> Squares;

        [Header("Prefabs")]
        public GameObject RowPrefab;
        public UI_RankingSquare ElementPrefab;

        [Header("Elements")]
        public GameObject RowContainer;


        public void Init(int populationSize)
        {
            Squares = new List<UI_RankingSquare>();

            HelperFunctions.DestroyAllChildredImmediately(RowContainer);

            float gridWidth = GetComponent<RectTransform>().rect.width;

            int elementsPerRow = (int)(gridWidth / 14);

            GameObject currentRow = null;
            for (int i = 0; i < populationSize; i++)
            {
                if (i % elementsPerRow == 0)
                {
                    currentRow = GameObject.Instantiate(RowPrefab, RowContainer.transform);
                    HelperFunctions.DestroyAllChildredImmediately(currentRow);
                }
                Squares.Add(GameObject.Instantiate(ElementPrefab, currentRow.transform));
            }

            RowContainer.SetActive(false);
        }

        public void ShowGeneration(IncrementumGenerationStats stats)
        {
            if (stats.TaskRanking == null || stats.TaskRanking.Count == 0)
            {
                RowContainer.SetActive(false);
                return;
            }

            RowContainer.SetActive(true);

            for(int i = 0; i < stats.TaskRanking.Count; i++)
            {
                Squares[i].Init((IncrementumTask)stats.TaskRanking[i]);
            }
        }
    }
}
