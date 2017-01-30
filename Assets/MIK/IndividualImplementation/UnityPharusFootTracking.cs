using UnityEngine;

namespace ModularIK {
    class UnityPharusFootTracking : ATrackingEntity {

        private Color defautColor = new Color(0, 1, 0, 1);

        public Transform playerFootPrefab;
        public Transform leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure

        private LeftFootData leftFootData; //tempFootData; TODO: find a solution for temtFootData (Maybe not needed bacause the processing happens earlier)
        private RightFootData rightFootData;
        private HipData hipData;
        private Vector3 leftFootPos, leftFootLastPos, rightFootPos, rightFootLastPos, tempFootPos;
        private Vector3 hipPos;

        public bool filterFeet = true;
        [Range(1.0f, 0.0000001f)]
        public double kalmanQ = 0.00005;
        [Range(1.0f, 0.0001f)]
        public double kalmanR = 0.01;

        SimpleKalman[] filters;

        private float _scaling = 0.01f;

        #region unity callbacks
        private void Start() {
            filters = new SimpleKalman[4];
            for (int i = 0; i < filters.Length; i++)
            {
                filters[i] = new SimpleKalman();
                filters[i].Q = kalmanQ;
                filters[i].R = kalmanR;
            }

            leftFoot = Instantiate(playerFootPrefab);
            rightFoot = Instantiate(playerFootPrefab);
            tempFoot = Instantiate(playerFootPrefab);
            tempFoot.GetComponent<MeshRenderer>().enabled = false;

            leftFoot.parent = transform;
            rightFoot.parent = transform;
            tempFoot.parent = transform;

            InitFeet();
        }

        private void Update()
        {
            TrackFeet();
            if (filterFeet)
                FilterFeet();

            CalculateHip();
            CheckEchos();

            ShowFootTraces(Color.red, Color.blue);
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
            leftFootData = new LeftFootData(() => GetLeftFootPosition(), () => GetCenterRotation()/*GetLeftFootRotation()*/);
            rightFootData = new RightFootData(() => GetRightFootPosition(), () => GetCenterRotation()/*GetRightFootRotation()*/);
            hipData = new HipData(() => GetCenterPosition(),() => GetCenterRotation());

            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.AddProvider(leftFootData);
            mdm.AddProvider(rightFootData);
            mdm.AddProvider(hipData);
        }
        private float[] GetLeftFootPosition() {
            return new float[] { leftFootPos.x, leftFootPos.y, leftFootPos.z };
        }
        private float[] GetLeftFootRotation()
        {
            Vector3 dir = leftFootPos - leftFootLastPos;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightFootRotation()
        {
            Vector3 dir = rightFootPos - rightFootLastPos;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightFootPosition() {
            return new float[] { rightFootPos.x, rightFootPos.y, rightFootPos.z };
        }
        // TODO: consider to provide a non-monobehavior solution which approximates the hip position based on foot data
        private float[] GetCenterPosition()
        {
            return new float[] { hipPos.x, hipPos.y, hipPos.z };
        }

        // TODO: this is the smoothed orientation of pharus
        private float[] GetCenterRotation()
        {
            return new float[] { Orientation.x, 0, Orientation.y };
        }

        private void TrackFeet() {
            if (Echoes == null)
                return;

            leftFootLastPos = leftFootPos;
            rightFootLastPos = rightFootPos;

            if (Echoes.Count > 1) { // found 2 feet
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[1].x, Echoes[1].y);
                // TODO: get rid of "/ _scaling" at foot height
                rightFoot.position = new Vector3(pos.x, rightFoot.position.y / _scaling, pos.y) * _scaling;
                pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                leftFoot.position = new Vector3(pos.x, leftFoot.position.y / _scaling, pos.y) * _scaling;

                // feet are crossed
                if (leftFoot.localPosition.x > rightFoot.localPosition.x) {
                    Vector3 temp = leftFoot.position;
                    leftFoot.position = rightFoot.position;
                    rightFoot.position = temp;
                }
            }
            else if (Echoes.Count > 0) {  // found 1 foot
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                tempFoot.position = new Vector3(pos.x, tempFoot.position.y / _scaling, pos.y) * _scaling;
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

        private void FilterFeet()
        {
            foreach (SimpleKalman item in filters)
            {
                item.Q = kalmanQ;
                item.R = kalmanR;
            }

            leftFoot.position = new Vector3((float)filters[0].UseFilter(leftFoot.position.x), leftFoot.position.y, (float)filters[1].UseFilter(leftFoot.position.z));
            rightFoot.position = new Vector3((float)filters[2].UseFilter(rightFoot.position.x), rightFoot.position.y, (float)filters[3].UseFilter(rightFoot.position.z));

            leftFootPos = leftFoot.position;
            rightFootPos = rightFoot.position;
        }

        private void CalculateHip()
        {
            hipPos = (rightFootPos + leftFootPos) / 2;
        }

        private void ShowFootTraces(Color l, Color r)
        {
            Debug.DrawLine(leftFootLastPos, leftFootPos, l, 2);
            Debug.DrawLine(rightFootLastPos, rightFootPos, r, 2);
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