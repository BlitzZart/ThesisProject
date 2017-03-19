using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MIKA {
    [NetworkSettings(channel = 2)]
    public class NetworkPlayer : NetworkBehaviour, ICenterReceiver, ILeftFootReceiver, IRightFootReceiver, ILeftHandReceiver, IRightHandReceiver, IHeadReceiver {

        private float centerLerpSpeed = 13;
        public Transform leftFoot, rightFoot;

        [SyncVar]
        private Vector3     centerPosition, leftFootPosition, rightFootPosition, leftHandPosition, rightHandPosition, lookAtTarget;
        [SyncVar]
        private Vector3     leftFootRotation, rightFootRotation, leftHandRotation, rightHandRotation;
        [SyncVar]
        private Quaternion  centerRrotation;

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
                modelDataManager = FindObjectOfType<UnityModelDataManager>();
                modelDataManager.SubscribeReceiver(this);

                StartCoroutine(TryAssignFeet());
            } else if (isLocalPlayer) {
                OVRCameraRig ovr = FindObjectOfType<OVRCameraRig>();

                if (ovr != null) {
                    ovr.transform.parent = transform;
                }
            }
        }

        void Update() {
            if (isServer) {
                // TODO: not for own character
                // but has to be done on clients too. for opposite character
                ProcessIK();

            } else if (isLocalPlayer) {
                UpdateOwnTransformations();
            }

        }

        private void OnDestroy() {
            if (isServer)
                modelDataManager.UnsubscribeReseiver(this);
        }
        #endregion

        #region private
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

        #endregion

        #region DataReceiver
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
            lookAtTarget = new Vector3(position[0], position[1], position[2]);
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
    }
}