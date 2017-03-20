using UnityEngine;
using System.Collections;

namespace MIKA {
    // offline solution
    // has been used for testing without network connetion to the headset
    public class GearVRHead : MonoBehaviour {
        public Transform lookAtTarget;
        private HeadData headData;

        private void Start() {
            InitHead();
            StartCoroutine(PollModelDataManager());
        }

        private void OnDestroy() {
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            if (mdm != null)
                mdm.RemoveProvider(headData);
        }

        IEnumerator PollModelDataManager() {
            UnityModelDataManager mdm = FindObjectOfType<UnityModelDataManager>();
            while (mdm == null) {
                mdm = FindObjectOfType<UnityModelDataManager>();
                yield return new WaitForSeconds(0.7f);
            }
            mdm.AddProvider(headData);
        }

        private void InitHead() {
            headData = new HeadData(() => GetHeadPosition(), () => GetHeadRotation());
        }

        // this is the look at position
        private float[] GetHeadPosition() {
            return new float[] { lookAtTarget.position.x, lookAtTarget.position.y, lookAtTarget.position.z };
        }

        private float[] GetHeadRotation() {
            return new float[] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z };
        }
    }
}