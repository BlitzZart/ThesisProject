using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA
{
    public class VivePlayerManager : Singleton<VivePlayerManager> {

        public UnityViveTracking trackedPrefab;
        public UnityViveTracking trackedEntity1;
        public UnityViveTracking trackedEntity2;

        public List<IMikaTrackedEntity> entities; 

        void Start() {
            StartCoroutine(CheckForPlayer());
        }

        void Update() {

        }

        private IEnumerator CheckForPlayer()
        {
            // get first one - only used for recording
            while (true) {
                print("Search trackers");
                yield return new WaitForSeconds(0.2f);

                if (trackedEntity1 == null) {
                    GameObject t1 = GameObject.FindGameObjectWithTag("ViveTracker01");
                    GameObject t2 = GameObject.FindGameObjectWithTag("ViveTracker02");

                    if (t1 && t2) {
                        trackedEntity1 = Instantiate(trackedPrefab.gameObject).GetComponent<UnityViveTracking>();
                        trackedEntity1.Initialize(t1.transform, t2.transform);
                    }
                }
                yield return new WaitForSeconds(0.1f);
                if (trackedEntity2 == null) {
                    GameObject t1 = GameObject.FindGameObjectWithTag("ViveTracker03");
                    GameObject t2 = GameObject.FindGameObjectWithTag("ViveTracker04");

                    if (t1 && t2) {
                        trackedEntity2 = Instantiate(trackedPrefab.gameObject).GetComponent<UnityViveTracking>();
                        trackedEntity2.Initialize(t1.transform, t2.transform);
                    }
                }
            }

            //entities.Add(trackedEntity1);
            //while (true) {
            //    yield return new WaitForSeconds(0.2f);
            //    print("Search for further players");
            //}

        }
    }
}