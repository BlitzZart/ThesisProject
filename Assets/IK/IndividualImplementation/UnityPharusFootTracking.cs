using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace ModularIK {
    class UnityPharusFootTracking : ATrackingEntity {
        public Transform playerFootPrefab, body, correctedCenter;
        public Material[] _feetMaterials;
        public Transform leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure

        //
        private FootData leftFootData, rightFootData, tempFootData;
        private Vector3 leftFootPos, rightFootPos, tempFootPos;
        //

        private Transform ikBody;

        public float Height;

        private float _scaling = 0.01f;

        #region unity callbacks
        void Start() {
            leftFoot = Instantiate(playerFootPrefab);
            rightFoot = Instantiate(playerFootPrefab);
            tempFoot = Instantiate(playerFootPrefab);
            tempFoot.GetComponent<MeshRenderer>().enabled = false;

            leftFoot.parent = transform;
            rightFoot.parent = transform;
            tempFoot.parent = transform;

            leftFoot.GetComponent<MeshRenderer>().material = _feetMaterials[0];
            rightFoot.GetComponent<MeshRenderer>().material = _feetMaterials[1];

            AssignIKModel();
            AssignAIAgent(body);
        }

        void Update() {
            TrackFeet();
            SetAvatarPosition();
            SetAvatarOrientation();

            CheckEchos();
        }
        #endregion


        private void InitFeet() {
            leftFootData = new FootData(() => GetLeftFootPosition(), null);
            rightFootData = new FootData(() => GetRightFootPosition(), null);
        }
        private float[] GetLeftFootPosition() {
            return new float[] { leftFootPos.x, leftFootPos.y, leftFootPos.z };
        }
        private float[] GetRightFootPosition() {
            return new float[] { rightFootPos.x, rightFootPos.y, rightFootPos.z };
        }

        #region public

        public override void SetPosition(Vector2 coords) {
            //this.transform.position = coords * _scaling;
            this.transform.position = new Vector3(coords.x, 0, coords.y) * _scaling;
        }
        #endregion

        #region private
        private void SetAvatarPosition() {
            ikBody.position = transform.position;
            ikBody.rotation = Quaternion.LookRotation(new Vector3(Orientation.x, 0, Orientation.y));
        }

        private void SetAvatarOrientation() {
            Vector3 dir = new Vector3(Orientation.x, 0, Orientation.y);
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        private void AssignIKModel() {
            IKControl ik = FindObjectOfType<IKControl>();
            if (ik == null)
                return;

            if (ik.rightFootObj || ik.leftFootObj)
                return;

            ikBody = ik.transform;

            ik.rightFootObj = rightFoot;
            ik.leftFootObj = leftFoot;
        }

        private void AssignAIAgent(Transform target) {
            AICharacterControl ai = FindObjectOfType<AICharacterControl>();

            if (ai == null)
                return;

            ai.SetTarget(target);
        }

        private void TrackFeet() {
            if (Echoes == null)
                return;
            if (Echoes.Count > 1) { // found 2 feet
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[1].x, Echoes[1].y);
                rightFoot.position = new Vector3(pos.x, rightFoot.position.y, pos.y) * _scaling;
                pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                leftFoot.position = new Vector3(pos.x, leftFoot.position.y, pos.y) * _scaling;

                // feet are crossed
                if (leftFoot.localPosition.x > rightFoot.localPosition.x) {
                    Vector3 temp = leftFoot.position;
                    leftFoot.position = rightFoot.position;
                    rightFoot.position = temp;

                }
            }
            else if (Echoes.Count > 0) {  // found 1 foot
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                tempFoot.position = new Vector3(pos.x, tempFoot.position.y, pos.y) * _scaling;
                if (tempFoot.localPosition.x < 0) {
                    leftFoot.position = tempFoot.position;
                }
                else {
                    rightFoot.position = tempFoot.position;
                }
            }

            // NEW ++++
            leftFootPos = leftFoot.position;
            rightFootPos = rightFoot.position;
        }

        private void CheckEchos() {
            if (Echoes.Count < 1) {
                Debug.LogError("lost both foot echos");
                //print("lost both foot echos");
            }
            else if (Echoes.Count < 2) {
                Debug.LogError("lost one foot echo");
                //print("lost one foot echo");
            }
        }
        #endregion
    }
}
