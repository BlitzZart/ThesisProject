using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace MIKA {
    [NetworkSettings(channel = 2)]
    public class NetworkPlayer : NetworkBehaviour, ICenterReceiver, ILeftFootReceiver, IRightFootReceiver, ILeftHandReceiver, IRightHandReceiver, IHeadReceiver {
        private float eyeHeight = 1.65f;
        private float centerLerpSpeed = 13;
        private OVRCameraRig ovr;
        public Transform leftFoot, rightFoot;

        public GameObject vrFootPrefab;

        // data provider
        private HeadData headData;

        // data receiver
        [SyncVar]
        private Vector3     centerPosition, leftFootPosition, rightFootPosition, leftHandPosition, rightHandPosition, lookAtTarget;
        [SyncVar]
        private Vector3     leftFootRotation, rightFootRotation, leftHandRotation, rightHandRotation;
        [SyncVar]
        private Quaternion  centerRrotation;

        // ---- CLIENT ----
        private GearVRHead vrHead;

        // ---- SERVER ----
        private UnityModelDataManager modelDataManager;
        private IKControl ikControl;

        //public IKModelController(IModelDataManager mdm, IKControl ik) {
        //    modelDataManager = mdm;
        //    modelDataManager.SubscribeReceiver(this);

        //    AssignIKModel(ik);
        //}

        #region unity callbacks
        void Start() {
            if (isServer) {
                SetUpServer();
            } else if (isLocalPlayer) {
                SetUpLocalPlayer();
            }
        }

        void Update() {
            if (isServer) {
                UpdateServer();
            } else if (isLocalPlayer) {
                UpdateLocalPlayer();
            }
        }

        private void OnDestroy() {
            if (modelDataManager != null) {
                modelDataManager.RemoveProvider(headData);
                modelDataManager.UnsubscribeReseiver(this);

                headData = null;
            }
        }
        #endregion

        #region private
        // ---- CLIENT ----
        private void SetUpLocalPlayer() {
            // set OVR camera to networkplayer position
            ovr = FindObjectOfType<OVRCameraRig>();
            if (ovr != null) {
                ovr.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                ovr.transform.parent = transform;
                vrHead = ovr.GetComponent<GearVRHead>();
            }
            SetUpLocalFeet();
        }
        private void SetUpLocalFeet() {
            leftFoot = Instantiate(vrFootPrefab).transform;
            rightFoot = Instantiate(vrFootPrefab).transform;
        }
        private void UpdateLocalPlayer() {
            UpdateOwnTransformations();
            CmdSetLookAtTarget(vrHead.lookAtTarget.position);
            UpdateLocalFeet();
        }
        private void UpdateLocalFeet() {
            leftFootPosition.y = 0;
            leftFoot.position = Vector3.Lerp(leftFoot.position, leftFootPosition, 20 * Time.deltaTime);
            rightFootPosition.y = 0;
            rightFoot.position = Vector3.Lerp(rightFoot.position, rightFootPosition, 20 * Time.deltaTime); ;
        }

        // ---- SERVER ----
        private void SetUpServer() {
            InitHead();

            modelDataManager = FindObjectOfType<UnityModelDataManager>();
            modelDataManager.AddProvider(headData);

            modelDataManager.SubscribeReceiver(this);

            StartCoroutine(TryAssignFeet());
        }
        private void UpdateServer() {
            // TODO: not for own character
            // but has to be done on clients too. for opposite character
            UpdateOwnTransformations();
            ProcessIK();
        }
        private void AssignIKModel(IKControl ikControl) {
            this.ikControl = ikControl;

            //if (this.ikControl == null)
            //    return;
            //ikBody = this.ikControl.transform;
            //if (this.ikControl.rightFootObj || this.ikControl.leftFootObj)
            //    return;
            //ik.rightFootObj = rightFoot;
            //ik.leftFootObj = leftFoot;

        }
        private IEnumerator TryAssignFeet() {
            while (leftFoot == null ||rightFoot == null) {
                UnityPharusFootTracking go = FindObjectOfType<UnityPharusFootTracking>();
                if (go != null) {
                    leftFoot = go.LeftFoot.transform;
                    rightFoot = go.RightFoot.transform;
                }

                yield return new WaitForSeconds(0.333f);
            }
        }
        private void UpdateOwnTransformations() {
            Vector3 pos = centerPosition;
            pos.y = 1.65f;
            transform.position = Vector3.Lerp(transform.position, pos, centerLerpSpeed * Time.deltaTime);
        }

        #endregion

        #region networking
        [Command]
        private void CmdSetLookAtTarget(Vector3 target) {
            if (!isServer)
                return;

            lookAtTarget = target;
        }
        #endregion

        #region Data provider
        //IEnumerator PollModelDataManager() {
        //    UnityModelDataManager mdm = FindObjectOfType<UnityModelDataManager>();
        //    while (mdm == null) {
        //        yield return new WaitForSeconds(0.7f);
        //        mdm = FindObjectOfType<UnityModelDataManager>();
        //    }
        //    mdm.AddProvider(headData);
        //}

        private void InitHead() {
            headData = new HeadData(() => GetHeadPosition(), () => GetHeadRotation());
        }

        private float[] GetHeadPosition() {
            return new float[] { lookAtTarget.x, lookAtTarget.y, lookAtTarget.z };
        }

        private float[] GetHeadRotation() {
            if (this == null)
                return null;

            Vector3 r = (lookAtTarget - transform.position).normalized;

            return new float[] { r.x, r.y, r.z };
        }

        #endregion

        #region Data receiver
        // TODO: get rid of this implementation
        void IDataReceiver.VectorData(float[] position, float[] rotation) {
            // DONT USE ME
        }

        void ICenterReceiver.VectorData(float[] position, float[] rotation) {
            centerPosition = new Vector3(position[0], position[1], position[2]);
            centerRrotation = Quaternion.LookRotation(new Vector3(rotation[0], rotation[1], rotation[2]), Vector3.up);
        }
        void ILeftFootReceiver.VectorData(float[] position, float[] rotation) {
            leftFootPosition = new Vector3(position[0], position[1], position[2]);
            rightFootRotation = Quaternion.LookRotation(new Vector3(rotation[0], rotation[1], rotation[2]), Vector3.up).eulerAngles;
        }
        void IRightFootReceiver.VectorData(float[] position, float[] rotation) {
            rightFootPosition = new Vector3(position[0], position[1], position[2]);
            leftFootRotation = Quaternion.LookRotation(new Vector3(rotation[0], rotation[1], rotation[2]), Vector3.up).eulerAngles;
        }
        void ILeftHandReceiver.VectorData(float[] position, float[] rotation) {
            leftHandPosition = new Vector3(position[0], position[1], position[2]);
            leftHandRotation = Quaternion.LookRotation(new Vector3(rotation[0], rotation[1], rotation[2]), Vector3.up).eulerAngles;
        }
        void IRightHandReceiver.VectorData(float[] position, float[] rotation) {
            rightHandPosition = new Vector3(position[0], position[1], position[2]);
            rightHandRotation = Quaternion.LookRotation(new Vector3(rotation[0], rotation[1], rotation[2]), Vector3.up).eulerAngles;
        }
        void IHeadReceiver.VectorData(float[] position, float[] rotation) {
            // client provides this information

            // TODO TODO TODO: activate this code!!!
            // lookAtTarget = new Vector3(position[0], position[1], position[2]);
        }

        void HipHeightApproximation() {

        }
        #endregion

        #region ik processing
        private void ProcessIK() {
            // TODO: this check is only for testing
            if (ikControl == null)
                return;

            ikControl.SetPosition(centerPosition);
            ikControl.SetRotation(centerRrotation);

            ikControl.leftFootPosition = leftFootPosition;
            ikControl.rightFootRotation = leftFootRotation;

            ikControl.rightFootPosition = rightFootPosition;
            ikControl.leftFootRotation = rightFootRotation;

            ikControl.leftHandPosition = leftHandPosition;
            ikControl.leftHandRotation = leftHandRotation;

            ikControl.rightHandPosition = rightHandPosition;
            ikControl.rightHandRotation = leftHandRotation;

            ikControl.lookAtTarget = lookAtTarget;
        }
        #endregion

        #region debug
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lookAtTarget, 0.04f);
        }
        #endregion
    }
}