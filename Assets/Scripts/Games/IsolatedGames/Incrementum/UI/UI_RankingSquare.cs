using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Incrementum
{
    public class UI_RankingSquare : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        bool isShowingTaskHistory;

        [Header("Elements")]
        public Image Square;

        public GameObject HistoryPanel;
        public TextMeshProUGUI HistoryText;

        public void Init(IncrementumTask task)
        {
            Square.color = task.Subject.Genome.Species.Color;
            HistoryText.text = task.GetHistoryLog();
            HistoryPanel.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HistoryPanel.SetActive(true);
            isShowingTaskHistory = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HistoryPanel.SetActive(false);
            isShowingTaskHistory = false;
        }
    }
}
