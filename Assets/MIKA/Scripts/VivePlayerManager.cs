using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA
{
    public class VivePlayerManager : Singleton<VivePlayerManager> {

        public UnityViveTracking trackedPrefab;
        public UnityViveTracking trackedEntity;

        void Start() {
            StartCoroutine(CheckForPlayer());
        }

        void Update() {

        }

        private IEnumerator CheckForPlayer()
        {
            while (trackedEntity == null)
            {
                yield return new WaitForSeconds(0.2f);
                print("Search player");
                if (FindObjectOfType<NetworkPlayer>() != null)
                {
                    print("Found Player");
                    trackedEntity = Instantiate(trackedPrefab.gameObject).GetComponent<UnityViveTracking>();
                }
            }
        }
    }
}