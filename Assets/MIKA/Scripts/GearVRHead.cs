using UnityEngine;
using System.Collections;

namespace MIKA {
    // offline solution
    // has been used for testing without network connetion to the headset
    public class GearVRHead : MonoBehaviour {
        public Transform lookAtTarget;
        private HeadData headData;
        private OVRCameraRig cameraRig;

        #region unity callbacks
        private void Start() {
            cameraRig = GetComponent<OVRCameraRig>();
            InitHead();
            StartCoroutine(PollModelDataManager());
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ResetRotation(cameraRig);

                Vector3 euler = transform.rotation.eulerAngles;
                euler.y = 0;
                transform.rotation = Quaternion.Euler(euler);
            }
        }
        private void OnDestroy() {
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            if (mdm != null)
                mdm.RemoveProvider(headData);
        }
        #endregion

        #region private
        private void ResetRotation(OVRCameraRig rig) {
            Transform root = cameraRig.trackingSpace;
            Transform centerEye = cameraRig.centerEyeAnchor;

            //Vector3 prevPos = root.position;
            Quaternion prevRot = root.rotation;

            transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

            //root.position = prevPos;
            root.rotation = prevRot;
        }
        private IEnumerator PollModelDataManager() {
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
        #endregion
    }
}