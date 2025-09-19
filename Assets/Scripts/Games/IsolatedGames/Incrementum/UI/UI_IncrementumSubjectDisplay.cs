using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Incrementum
{
    public class UI_IncrementumSubjectDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        bool isShowingTaskHistory;

        [Header("Elements")]
        public TextMeshProUGUI SubjectNameText;
        public TextMeshProUGUI FitnessText;
        public Image SpeciesColorKnob;

        public GameObject TaskHistory;
        public TextMeshProUGUI TaskHistoryText;

        public void Show(IncrementumTask task)
        {
            gameObject.SetActive(true);

            SubjectNameText.text = task.Subject.Name;
            SpeciesColorKnob.color = task.Subject.Genome.Species.Color;
            FitnessText.text = task.GetFitnessValue().ToString();
            TaskHistoryText.text = task.GetHistoryLog();

            TaskHistory.SetActive(isShowingTaskHistory);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TaskHistory.SetActive(true);
            isShowingTaskHistory = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TaskHistory.SetActive(false);
            isShowingTaskHistory = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isShowingTaskHistory = false;
        }
    }
}
