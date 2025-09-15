using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public class SimulationStat : MonoBehaviour
    {
        public SimulationStatId Id { get; private set; }
        public string DisplayName { get; private set; }
        public List<float> Values { get; private set; }


        /// <summary>
        /// Results of the current generation, used to determine an average throughout all rounds.
        /// </summary>
        private List<float> CurrentGenValues = new List<float>();

        public SimulationStat(SimulationStatId id, string name)
        {
            Id = id;
            DisplayName = name;
            Values = new List<float>();
        }

        public void AddNewValue()
        {
            Values.Add(0);
            CurrentGenValues.Clear();
        }

        public void AddCurrentGenValue(float value)
        {
            CurrentGenValues.Add(value);
            Values[Values.Count - 1] = CurrentGenValues.Average();
        }

        public float CurrentValue => Values.Count == 0 ? 0 : Values[Values.Count - 1];
        public string GetCurrentValueString()
        {
            float value = CurrentValue;
            string format = "";
            if (value <= 1) format = "N2";
            else if (value <= 100) format = "N1";
            else format = "N0";
            return value.ToString(format);
        }
    }
}
