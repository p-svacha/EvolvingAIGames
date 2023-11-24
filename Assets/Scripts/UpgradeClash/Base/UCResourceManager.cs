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
        public Sprite BasicShotIcon;

        public void Start()
        {
            _Singleton = GameObject.Find("ResourceManager").GetComponent<UCResourceManager>();
        }
    }
}
