using UnityEngine;

namespace ModularIK {
    class UnityPharusFootTracking : ATrackingEntity {
        public Transform playerFootPrefab;
        public Transform leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure

        private LeftFootData leftFootData; //tempFootData; TODO: find a solution for temtFootData (Maybe not needed bacause the processing happens earlier)
        private RightFootData rightFootData;
        private Vector3 leftFootPos, rightFootPos, tempFootPos;

        SimpleKalman kalman;
        private float _scaling = 0.01f;

        #region unity callbacks
        private void Start() {
            kalman = new SimpleKalman();
            kalman.Q = 0.025;

            leftFoot = Instantiate(playerFootPrefab);
            rightFoot = Instantiate(playerFootPrefab);
            tempFoot = Instantiate(playerFootPrefab);
            tempFoot.GetComponent<MeshRenderer>().enabled = false;

            leftFoot.parent = transform;
            rightFoot.parent = transform;
            tempFoot.parent = transform;

            InitFeet();
        }

        private void Update() {
            TrackFeet();
            //FilterFeet();
            CheckEchos();
        }

        private void OnDestroy() {
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.RemoveProvider(leftFootData);
            mdm.RemoveProvider(rightFootData);
        }
        #endregion

        #region public
        public override void SetPosition(Vector2 coords) {
            // transform tracking data. flip y and z axis and scale (1 unit = 1 meter)
            transform.position = new Vector3(coords.x, 0, coords.y) * _scaling;
        }
        #endregion

        #region private

        private void InitFeet() {
            leftFootData = new LeftFootData(() => GetLeftFootPosition(), null);
            rightFootData = new RightFootData(() => GetRightFootPosition(), null);

            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.AddProvider(leftFootData);
            mdm.AddProvider(rightFootData);
        }
        private float[] GetLeftFootPosition() {
            return new float[] { leftFootPos.x, leftFootPos.y, leftFootPos.z };
        }
        private float[] GetRightFootPosition() {
            return new float[] { rightFootPos.x, rightFootPos.y, rightFootPos.z };
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

            // set 
            leftFootPos = leftFoot.position;
            rightFootPos = rightFoot.position;
        }

        private void FilterFeet() {
            leftFoot.position = new Vector3((float)kalman.UseFilter(leftFoot.position.x), leftFoot.position.y, (float)kalman.UseFilter(leftFoot.position.z));
            rightFoot.position = new Vector3((float)kalman.UseFilter(rightFoot.position.x), rightFoot.position.y, (float)kalman.UseFilter(rightFoot.position.z));
        }

        private void CheckEchos() {
            if (Echoes.Count < 1) {
                Debug.LogError("lost both foot echos");
            }
            else if (Echoes.Count < 2) {
                Debug.LogError("lost one foot echo");
            }
        }
        #endregion
    }
}