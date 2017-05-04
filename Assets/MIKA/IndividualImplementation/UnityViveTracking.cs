using System;
using UnityEngine;

namespace MIKA {
    public class UnityViveTracking : MonoBehaviour, IHeadReceiver {
        public AnimationCurve footRotationCruve;
        [Range(0, 2)]
        public float distanceBetweenFeet = 0.0f;

        public GameObject avatarPrefab;
        private GameObject avatar;

        public Transform viveLeftFoot, viveRightFoot;
        public Vector3 leftFootPositionCorrection, rightFootPositionCorrection, leftFootDirectionCorrection, rightFootDirectionCorrection;  

        private bool walksBackwards = false;

        private LeftFootData leftFootData; //tempFootData; TODO: find a solution for tempFootData (Maybe not needed bacause the processing happens earlier)
        private RightFootData rightFootData;
        private LeftHandData leftHandData;
        private RightHandData rightHandData;

        private HipData hipData;
        private Vector3 hiddenLeftFootPosition, hiddenRightFootPosition;
        private Vector3 leftFootPosition, lastLeftFootPosition, rightFootPosition, lastRightFootPosition, tempFootPos;
        private Vector3 leftFootDirection, rightFootDirection;
        private Vector3 leftHandPosition, lastLeftHandPosition, rightHandPosition, lastRightHandPosition;
        private Vector3 centerPosition, lastCenterPosition;
        private Vector2 fastOrientation;
        private Vector3 headDirection;

        private bool useLineRenderers = true;

        public bool applyFilterOnFeet = true;
        public bool correctCrossing = true;
        [Range(0.1f, 0.0000001f)]
        public double kalmanQ = 0.00005;
        [Range(0.1f, 0.00001f)]
        public double kalmanR = 0.01;

        // 0: left x, 1: left y
        // 2: right x, 3: right y
        SimpleKalman[] kalmanFilters;
        // 0: orientatino x, 1: orientation y
        SimpleKalman[] orientationFilter;
        // filter hip height
        SimpleKalman hipHeightFilter;

        private float _scaling = 0.01f;

        #region unity callbacks
        private void Awake() {
            avatar = Instantiate(avatarPrefab);
        }
        private void Start() {
            // init filter
            kalmanFilters = new SimpleKalman[4];
            for (int i = 0; i < kalmanFilters.Length; i++) {
                kalmanFilters[i] = new SimpleKalman();
                kalmanFilters[i].Q = kalmanQ;
                kalmanFilters[i].R = kalmanR;
            }

            orientationFilter = new SimpleKalman[2];
            for (int i = 0; i < orientationFilter.Length; i++) {
                orientationFilter[i] = new SimpleKalman();
                orientationFilter[i].Q = 0.0001;
                orientationFilter[i].R = 0.005;
            }
            hipHeightFilter = new SimpleKalman();
            hipHeightFilter.Q = 0.0001;
            hipHeightFilter.R = 0.003;

            viveLeftFoot = GameObject.FindGameObjectWithTag("ViveTracker01").transform;// transform;
            viveRightFoot = GameObject.FindGameObjectWithTag("ViveTracker02").transform;

            InitFeet();

            // register data receiver becaus head rotation is needed here
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.SubscribeReceiver(this);
        }
        private void FixedUpdate() {
            FootTracking();
            //CheckEchos();
            //DrawTraces(Color.red, Color.blue, Color.green);
        }
        private void OnDestroy() {
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.RemoveProvider(leftFootData);
            mdm.RemoveProvider(rightFootData);
            mdm.RemoveProvider(hipData);
            mdm.RemoveProvider(leftHandData);
            mdm.RemoveProvider(rightHandData);

            mdm.SubscribeReceiver(this);
        }
        #endregion

        #region public
        #endregion

        #region data provider methods
        private void InitFeet() {
            leftFootData = new LeftFootData(() => GetLeftFootPosition(), () => GetLeftFootRotation()/*GetLeftFootRotation()*/);
            rightFootData = new RightFootData(() => GetRightFootPosition(), () => GetRightFootRotation()/*GetRightFootRotation()*/);
            hipData = new HipData(() => GetCenterPosition(), () => GetCenterRotation());
            leftHandData = new LeftHandData(() => GetLeftHandPosition(), () => GetLeftHandRotation());
            rightHandData = new RightHandData(() => GetRightHandPosition(), () => GetRightHandRotation());

            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.AddProvider(leftFootData);
            mdm.AddProvider(rightFootData);
            mdm.AddProvider(hipData);
            mdm.AddProvider(leftHandData);
            mdm.AddProvider(rightHandData);

            // comments are approximations which can be used if calibration with trackers does not provide satisfying results
            leftFootDirectionCorrection = /*vLeftFoot.up;*/new Vector3(0, -0.3f, 0.1f);
            leftFootDirectionCorrection.z = 0;
            rightFootDirectionCorrection = /*vRightFoot.up;*/new Vector3(0, -0.3f, 0.1f);
            rightFootDirectionCorrection.z = 0;

            print("Left foot orientation correction: " + leftFootDirectionCorrection);
            print("Right foot orientation correction: " + rightFootDirectionCorrection);


            lastLeftFootPosition = viveLeftFoot.position;
            lastRightFootPosition = viveRightFoot.position;

            TrackViveFeet();
        }
        private float CorrectFootHeightOffset() {
            float footHeightOffset = 0;
            float maxOffset = 0.07f;
            if (centerPosition.y < 0.0) {
                footHeightOffset = Mathf.Clamp(Mathf.Abs(centerPosition.y * 2.0f), 0, maxOffset);
            }
            return footHeightOffset;
        }

        private float[] GetLeftFootPosition() {
            //if (walksBackwards) {
            //    return new float[] { rightFootPosition.x, rightFootPosition.y, rightFootPosition.z };
            //}
            //else {
                return new float[] { leftFootPosition.x, leftFootPosition.y + CorrectFootHeightOffset(), leftFootPosition.z };
            //}
        }
        private float[] GetRightFootPosition() {
            //if (walksBackwards) {
            //    return new float[] { leftFootPosition.x, leftFootPosition.y, leftFootPosition.z };
            //}
            //else {
                return new float[] { rightFootPosition.x, rightFootPosition.y + CorrectFootHeightOffset(), rightFootPosition.z };
            //}
        }
        private float[] GetLeftFootRotation() {
            //return new float[] { leftFootDirection.x, leftFootDirection.y, leftFootDirection.z };
            return new float[] { rightFootDirection.x, rightFootDirection.y, rightFootDirection.z };
        }
        private float[] GetRightFootRotation() {
            return new float[] { leftFootDirection.x, leftFootDirection.y, leftFootDirection.z };
            //return new float[] { rightFootDirection.x, rightFootDirection.y, rightFootDirection.z };
        }
        private float[] GetCenterPosition() {
            // add a small offset in walking direction
            Vector3 centerWithOffset = centerPosition + avatar.transform.forward * 0.07f;
            return new float[] { centerWithOffset.x, centerPosition.y , centerWithOffset.z };
        }
        // Wheighted orientation - 40% left foot, 40% right foot and 20% head orientation
        private float[] GetCenterRotation() {
            Vector3 r = (leftFootDirection * 0.4f + rightFootDirection * 0.4f + headDirection * 0.2f);
            return new float[] { r.x, 0, r.z};
        }
        // hands
        private float[] GetLeftHandPosition() {
            leftHandPosition = Vector3.Lerp(leftHandPosition,
                                            rightFootPosition - avatar.transform.right * GetHandBodyDistance(rightFootPosition)
                                            - avatar.transform.up * 0.45f +avatar.transform.forward * 0.17f,
                                            15.0f * Time.fixedDeltaTime);

            return new float[] { leftHandPosition.x, -leftHandPosition.y, leftHandPosition.z };
        }
        private float[] GetRightHandPosition() {
            //rightHandPosition = Vector3.Lerp(lastRightFootPosition,
            //leftFootPosition + avatar.transform.right * 0.4f - avatar.transform.up * 0.35f + avatar.transform.forward * 0.17f,
            //65.0f * Time.fixedDeltaTime);
            rightHandPosition = Vector3.Lerp(rightHandPosition,
                                            leftFootPosition + avatar.transform.right * GetHandBodyDistance(leftFootPosition)
                                            - avatar.transform.up * 0.45f + avatar.transform.forward * 0.17f,
                                            15.0f * Time.fixedDeltaTime);

            return new float[] { rightHandPosition.x, -rightHandPosition.y, rightHandPosition.z };
        }
        private float GetHandBodyDistance(Vector3 footPos) {
            float minDistance = 0.43f;

            float distance = Vector3.Distance(leftFootPosition, rightFootPosition);
            float oneMinusDot =  1 - Mathf.Abs(Vector3.Dot(avatar.transform.forward, (centerPosition - leftFootPosition).normalized));

            return Mathf.Max(minDistance,(distance * (oneMinusDot)));
        }
        private float[] GetLeftHandRotation() {
            Vector3 dir = leftFootPosition - lastLeftFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightHandRotation() {
            Vector3 dir = rightFootPosition - lastRightFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }

        #endregion
        #region private
        // check if a point is on the left side of the center line (aligned to orientation)
        private bool IsLeftOfCenter(Vector2 c) {
            Vector2 a = new Vector2(centerPosition.x, centerPosition.z);
            Vector2 b = a + fastOrientation;
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
        }
        private void ReinitializeAllFilters() {
            foreach (SimpleKalman item in kalmanFilters)
                item.Reinitialize();
        }
        // TODO: clean up (temp...)
        Vector3 leftTemp, rightTemp;
        private void TrackViveFeet() {
            leftTemp = Vector3.Lerp(leftFootPosition, viveLeftFoot.position, 20 * Time.deltaTime);
            rightTemp = Vector3.Lerp(rightFootPosition, viveRightFoot.position, 20 * Time.deltaTime);

            leftFootDirection = viveLeftFoot.up;
            leftFootDirection = (leftFootDirection - leftFootDirectionCorrection).normalized;
            rightFootDirection = viveRightFoot.up;
            rightFootDirection = (rightFootDirection - rightFootDirectionCorrection).normalized;


            if (leftTemp.y < 0) leftTemp.y = 0;
            if (rightTemp.y < 0) rightTemp.y = 0;

            if (leftTemp.y > 2.0f) leftTemp.y = 1.2f;
            if (rightTemp.y > 2.0f) rightTemp.y = 1.2f;

                float leftDist = Mathf.Abs(Vector3.Distance(leftTemp, lastLeftFootPosition));
            float rightDist = Mathf.Abs(Vector3.Distance(rightTemp, lastRightFootPosition));

            if (leftDist > 0.1f) {
                Vector3.Lerp(leftFootPosition, viveLeftFoot.position, 0.5f * Time.deltaTime);
            }
            if (rightDist > 0.1f) {
                Vector3.Lerp(rightFootPosition, viveRightFoot.position, 0.5f * Time.deltaTime);
            }

            leftFootPosition = leftTemp;
            rightFootPosition = rightTemp;
        }
        
        //private void FootFiltering() {
        //    if (!applyFilterOnFeet)
        //        return;

        //    foreach (SimpleKalman item in kalmanFilters) {
        //        item.Q = kalmanQ;
        //        item.R = kalmanR;
        //    }

        //    leftFootPosition = new Vector3((float)kalmanFilters[0].UseFilter(leftFootPosition.x), leftFootPosition.y, (float)kalmanFilters[1].UseFilter(leftFootPosition.z));
        //    rightFootPosition = new Vector3((float)kalmanFilters[2].UseFilter(rightFootPosition.x), rightFootPosition.y, (float)kalmanFilters[3].UseFilter(rightFootPosition.z));
        //}
        //private void OrientationFiltering() {
        //    Vector3 tempFast = (centerPosition - lastCenterPosition);
        //    fastOrientation = new Vector2(tempFast.x, tempFast.z).normalized;
        //    //fastOrientation = (Orientation + fastOrientation) / 2;
        //    fastOrientation = new Vector2((float)orientationFilter[0].UseFilter(fastOrientation.x), (float)orientationFilter[1].UseFilter(fastOrientation.y));
        //}
        private void FootAndHipHeight() {
            distanceBetweenFeet = Vector3.Distance(leftFootPosition, rightFootPosition);
            centerPosition.y = -(float)hipHeightFilter.UseFilter(Mathf.Clamp(((distanceBetweenFeet - 0.5f)  * 0.5f), 0.0f, 1.0f) - 0.0f);
        }
        private void FootTracking() {
            // store last positions
            lastCenterPosition = centerPosition;
            lastLeftFootPosition = leftFootPosition;
            lastRightFootPosition = rightFootPosition;

            // update center position
            centerPosition = (leftFootPosition + rightFootPosition) / 2;
            centerPosition.y = 0;
            transform.position = centerPosition;

            TrackViveFeet();

            FootAndHipHeight();
        }

        void IHeadReceiver.VectorData(float[] position, float[] rotation) {

            headDirection = new Vector3(rotation[0], rotation[1], rotation[2]).normalized;


            Vector2 headOrientation2D = new Vector2(rotation[0], rotation[2]).normalized;
            //// get angle between movement vector and head direction vector
            //if (Vector2.Angle(Orientation, headOrientation2D) > 90) {
            //    walksBackwards = true;
            //}
            //else {
            //    walksBackwards = false;
            //}
        }

        void IDataReceiver.VectorData(float[] position, float[] rotation) {
            //throw new NotImplementedException();
        }
        #endregion
        #region debug drawing
        float lineDuration = 5;
        private void DrawTraces(Color l, Color r, Color c) {
            Debug.DrawLine(lastLeftFootPosition, leftFootPosition, l, lineDuration);
            Debug.DrawLine(lastRightFootPosition, rightFootPosition, r, lineDuration);
            Debug.DrawLine(lastCenterPosition, centerPosition, c, lineDuration);
            //Debug.DrawRay(centerPosition, new Vector3(Orientation.x, 0, Orientation.y) * 0.5f, Color.magenta);
            Debug.DrawRay(centerPosition, new Vector3(fastOrientation.x, 0, fastOrientation.y) * 0.5f, Color.white);

            if (useLineRenderers) {
                //leftFoot.AddFootTracePoint(leftFootPosition);
                //rightFoot.AddFootTracePoint(rightFootPosition);
            }
        }
        private void OnDrawGizmos() {
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(centerPosition, 0.2f);
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(leftFootPosition, 0.1f);
            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(rightFootPosition, 0.1f);
        }
        #endregion
    }
}