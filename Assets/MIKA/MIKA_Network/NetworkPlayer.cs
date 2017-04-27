using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace MIKA {
    [NetworkSettings(channel = 2, sendInterval = 0)] // sendInterval = 0 means that everytime a SyncVar changes an update will be sent
    public class NetworkPlayer : NetworkBehaviour, ICenterReceiver, ILeftFootReceiver, IRightFootReceiver, ILeftHandReceiver, IRightHandReceiver, IHeadReceiver {
        private float eyeHeight = 1.45f;
        private float centerLerpSpeed = 50; // affets ovr-camera position (is parented to NetworkPlayer)
        private OVRCameraRig ovr;
        private Transform leftFoot, rightFoot;

        public GameObject vrFootPrefab;
        public GameObject avatarPrefab;
        private GameObject avatar;

        // data provider
        private HeadData headData;

        // data receiver
        [SyncVar]
        private Vector3     centerPosition, leftFootPosition, rightFootPosition, leftHandPosition, rightHandPosition, lookAtTarget;
        [SyncVar]
        private Vector3     leftFootRotation, rightFootRotation, leftHandRotation, rightHandRotation;
        [SyncVar]
        private Quaternion  centerRrotation;
        [SyncVar]
        public int playerNumber;

        // ---- CLIENT ----
        private GearVRHead vrHead;
        // ---- SERVER ----
        private UnityModelDataManager modelDataManager;
        // ---- BOTH ----
        private IKControl ikControl;

        #region unity callbacks
        void Start() {
            if (isServer) {
                SetUpServer();
            } else if (isLocalPlayer) {
                SetUpLocalPlayer();
            } else if (isClient) {
                SetUpAvatar();
            }
        }
        void Update() {
            if (isServer) {
                UpdateServer();
            } else if (isLocalPlayer) {
                UpdateLocalPlayer();
            } else if (isClient) {
                ProcessIK();
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
            //SetUpAvatar();
        }
        private void SetUpLocalFeet() {
            leftFoot = Instantiate(vrFootPrefab).transform;
            rightFoot = Instantiate(vrFootPrefab).transform;
        }
        private void SetUpAvatar() {
            avatar = Instantiate(avatarPrefab);
            ikControl = avatar.GetComponent<IKControl>();
        }
        private void UpdateLocalPlayer() {
            UpdateOwnTransformations();
            CmdSetLookAtTarget(vrHead.lookAtTarget.position);
            UpdateLocalFeet();
            ProcessIK();
        }
        private void UpdateLocalFeet() {
            //leftFootPosition.y = 0;
            leftFoot.position = leftFootPosition;// Vector3.Lerp(leftFoot.position, leftFootPosition, 20 * Time.deltaTime);
            //rightFootPosition.y = 0;
            rightFoot.position = rightFootPosition;// Vector3.Lerp(rightFoot.position, rightFootPosition, 20 * Time.deltaTime); ;
        }

        // ---- SERVER ----
        private void SetUpServer() {
            InitHead();

            StartCoroutine(TryAssignPlayer());
            StartCoroutine(TryAssignFeet());
        }
        private void UpdateServer() {
            // TODO: not for own character
            // but has to be done on clients too. for opposite character
            UpdateOwnTransformations();
            ProcessIK();
        }
        private IEnumerator TryAssignPlayer() {
            while (modelDataManager == null) {
                UnityModelDataManager[] mdms = FindObjectsOfType<UnityModelDataManager>();

                foreach (UnityModelDataManager item in mdms) {
                    if (item.playerAssigned == 0) {
                        modelDataManager = item;
                        // TODO: this is just a workaround.
                        // vive implementation has no "UnityPharusFootTracking"
                        if (modelDataManager.GetComponent<UnityPharusFootTracking>() != null)
                            playerNumber = modelDataManager.playerAssigned = (int)modelDataManager.GetComponent<UnityPharusFootTracking>().TrackID;
                        else
                            playerNumber = modelDataManager.playerAssigned = 1;

                        modelDataManager.AddProvider(headData);
                        modelDataManager.SubscribeReceiver(this);
                    }
                }

                yield return new WaitForSeconds(0.333f);
            }
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

        // ---- BOTH ----
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