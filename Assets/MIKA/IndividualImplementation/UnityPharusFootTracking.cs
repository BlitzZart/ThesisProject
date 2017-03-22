using System;
using UnityEngine;

namespace MIKA
{
    class UnityPharusFootTracking : ATrackingEntity, IHeadReceiver
    {
        public bool walksBackwards = false;
        public GameObject avatarPrefab;

        public PlayerFoot playerFootPrefab;
        private PlayerFoot leftFoot, rightFoot;
        public PlayerFoot LeftFoot { get { return leftFoot; } }
        public PlayerFoot RightFoot { get { return rightFoot; } }

        private LeftFootData leftFootData; //tempFootData; TODO: find a solution for tempFootData (Maybe not needed bacause the processing happens earlier)
        private RightFootData rightFootData;
        private LeftHandData leftHandData;
        private RightHandData rightHandData;

        private HipData hipData;
        private Vector3 leftFootPosition, lastLeftFootPosition, rightFootPosition, lastRightFootPosition, tempFootPos;
        private Vector3 lastCenterPosition;
        private Vector3 centerPosition;
        private Vector3 hipPosition;
        private Vector2 fastOrientation;

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
            Instantiate(avatarPrefab);
        }

        private void Start()
        {
            // init filter
            kalmanFilters = new SimpleKalman[4];
            for (int i = 0; i < kalmanFilters.Length; i++)
            {
                kalmanFilters[i] = new SimpleKalman();
                kalmanFilters[i].Q = kalmanQ;
                kalmanFilters[i].R = kalmanR;
            }

            orientationFilter = new SimpleKalman[2];
            for (int i = 0; i < orientationFilter.Length; i++)
            {
                orientationFilter[i] = new SimpleKalman();
                orientationFilter[i].Q = 0.0001;
                orientationFilter[i].R = 0.005;
            }
            hipHeightFilter = new SimpleKalman();
            hipHeightFilter.Q = 0.0001;
            hipHeightFilter.R = 0.003;

            InitFeet();

            // register data receiver becaus head rotation is needed here
            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.SubscribeReceiver(this);
        }

        private void Update()
        {
            FootTracking();
            //TrackFeet();
            //if (filterFeet)
            //    FilterFeet();

            //CalculateHip();
            //CheckEchos();

            DrawTraces(Color.red, Color.blue, Color.green);
        }

        private void OnDestroy()
        {
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
        public override void SetPosition(Vector2 coords)
        {
            // transform tracking data. flip y and z axis and scale (1 unit = 1 meter)
            //transform.position = new Vector3(coords.x, 0, coords.y) * _scaling;
        }

        #endregion

        #region data provider methods
        private void InitFeet()
        {
            leftFootData = new LeftFootData(() => GetLeftFootPosition(), () => GetCenterRotation()/*GetLeftFootRotation()*/);
            rightFootData = new RightFootData(() => GetRightFootPosition(), () => GetCenterRotation()/*GetRightFootRotation()*/);
            hipData = new HipData(() => GetCenterPosition(), () => GetCenterRotation());
            leftHandData = new LeftHandData(() => GetLeftHandPosition(), () => GetLeftHandRotation());
            rightHandData = new RightHandData(() => GetRightHandPosition(), () => GetRightHandRotation());

            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.AddProvider(leftFootData);
            mdm.AddProvider(rightFootData);
            mdm.AddProvider(hipData);
            mdm.AddProvider(leftHandData);
            mdm.AddProvider(rightHandData);

            leftFoot = Instantiate(playerFootPrefab);
            rightFoot = Instantiate(playerFootPrefab);

            if (useLineRenderers)
            {
                leftFoot.InitLineRenderer(Color.red);
                rightFoot.InitLineRenderer(Color.blue);
            }
        }
        private float[] GetLeftFootPosition()
        {
            if (walksBackwards) {
                return new float[] { rightFootPosition.x, rightFootPosition.y, rightFootPosition.z };
            } else {
                return new float[] { leftFootPosition.x, leftFootPosition.y, leftFootPosition.z };
            }
        }
        private float[] GetLeftFootRotation()
        {
            Vector3 dir = leftFootPosition - lastLeftFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightFootRotation()
        {
            Vector3 dir = rightFootPosition - lastRightFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightFootPosition()
        {
            if (walksBackwards) {
                return new float[] { leftFootPosition.x, leftFootPosition.y, leftFootPosition.z };
            }
            else {
                return new float[] { rightFootPosition.x, rightFootPosition.y, rightFootPosition.z };
            }
        }
        // TODO: consider to provide a non-monobehavior solution which approximates the hip position based on foot data
        private float[] GetCenterPosition()
        {
            return new float[] { centerPosition.x, centerPosition.y + 0.07f, centerPosition.z };
        }
        // TODO: this is the smoothed orientation of pharus
        private float[] GetCenterRotation()
        {
            return (walksBackwards) ? new float[] { -Orientation.x, 0, -Orientation.y }: new float[] { Orientation.x, 0, Orientation.y };
        }
        // hands
        private float[] GetLeftHandPosition() {
            return new float[] { leftFootPosition.x, -leftFootPosition.y, leftFootPosition.z };
        }
        private float[] GetRightHandPosition() {
            return new float[] { rightFootPosition.x, -rightFootPosition.y, rightFootPosition.z };
        }
        private float[] GetLeftHandRotation() {
            Vector3 dir = leftFootPosition - lastLeftFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }
        private float[] GetRightHandRotation() {
            Vector3 dir = rightFootPosition - lastRightFootPosition;
            return new float[] { dir.x, dir.y, dir.z };
        }

        //private float[] GetRightHandRotation() {
        //    Vector3 dir = rightFootPosition - lastRightFootPosition;
        //    return new float[] { dir.x, dir.y, dir.z };
        //}
        //private float[] GetRightHandPosition() {
        //    return new float[] { rightFootPosition.x, rightFootPosition.y, rightFootPosition.z };
        //}

        #endregion
        #region private
        private void CheckEchos()
        {
            if (Echoes.Count < 1)
            {
                Debug.LogError("lost both foot echos");
            }
            else if (Echoes.Count < 2)
            {
                Debug.LogError("lost one foot echo");
            }
        }
        // check if a point is on the left side of the center line (aligned to orientation)
        private bool IsLeftOfCenter(Vector2 c)
        {
            Vector2 a = new Vector2(centerPosition.x, centerPosition.z);
            Vector2 b = a + fastOrientation;
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
        }

        private void ReinitializeAllFilters()
        {
            foreach (SimpleKalman item in kalmanFilters)
                item.Reinitialize();
        }
        private void TwoFeet()
        {
            // convert echoes to screen position
            Vector2 e0 = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
            Vector2 e1 = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[1].x, Echoes[1].y);

            // feet crossed check 1
            // check if left foot is left of center considering movement direction
            // TODO: combine with HMD orientation
            if (correctCrossing)
            {
                if (!IsLeftOfCenter(e0 * _scaling)/* || !IsLeftOfCenter(e1 * _scaling)*/)
                {
                    VectorHelpers.Swap(ref e0, ref e1);
                    //ReinitializeAllFilters();
                }
            }

            // scale to used space
            leftFootPosition = new Vector3(e0.x, 0, e0.y) * _scaling;
            rightFootPosition = new Vector3(e1.x, 0, e1.y) * _scaling;

            // feet crossed check 2
            // checking if new foot position is closer own last position than other foot last position
            if (correctCrossing)
            {
                if (Vector2.Distance(leftFootPosition, lastLeftFootPosition) > Vector2.Distance(leftFootPosition, lastRightFootPosition)
                 && Vector2.Distance(rightFootPosition, lastRightFootPosition) > Vector2.Distance(rightFootPosition, lastLeftFootPosition))
                {
                    VectorHelpers.Swap(ref leftFootPosition, ref rightFootPosition);
                    //ReinitializeAllFilters();
                }
            }

            //transform.position = (leftFootPosition + rightFootPosition) / 2;
        }
        private void OneFoot()
        {
            //// convert echo to screen position
            //Vector2 e0 = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);

            //// update proper foot
            //// check if the echo is left or right of center
            //if (IsLeftOfCenter(e0 * _scaling))
            //    leftFootPosition = new Vector3(e0.x, 0, e0.y) * _scaling;
            //else
            //    rightFootPosition = new Vector3(e0.x, 0, e0.y) * _scaling;
        }
        private void NoFoot()
        {

        }
        private void FootFiltering()
        {
            if (!applyFilterOnFeet)
                return;

            foreach (SimpleKalman item in kalmanFilters)
            {
                item.Q = kalmanQ;
                item.R = kalmanR;
            }

            leftFootPosition = new Vector3((float)kalmanFilters[0].UseFilter(leftFootPosition.x), leftFootPosition.y, (float)kalmanFilters[1].UseFilter(leftFootPosition.z));
            rightFootPosition = new Vector3((float)kalmanFilters[2].UseFilter(rightFootPosition.x), rightFootPosition.y, (float)kalmanFilters[3].UseFilter(rightFootPosition.z));
        }
        private void OrientationFiltering()
        {
            Vector3 tempFast = (centerPosition - lastCenterPosition);
            fastOrientation = new Vector2(tempFast.x, tempFast.z).normalized;
            //fastOrientation = (Orientation + fastOrientation) / 2;
            fastOrientation = new Vector2((float)orientationFilter[0].UseFilter(fastOrientation.x), (float)orientationFilter[1].UseFilter(fastOrientation.y));
        }
        private void FootAndHipHeight()
        {
            // get heights
            leftFootPosition.y = leftFoot.EstimateHeight(leftFootPosition, lastLeftFootPosition);
            rightFootPosition.y = rightFoot.EstimateHeight(rightFootPosition, lastRightFootPosition);
            // TODO: calculation is just hacked right now
            centerPosition.y =  (float)hipHeightFilter.UseFilter(((leftFootPosition.y + rightFootPosition.y) - 0.4f) * -1.0f);
        }
        private void FootTracking()
        {
            // store last positions
            lastCenterPosition = centerPosition;
            lastLeftFootPosition = leftFootPosition;
            lastRightFootPosition = rightFootPosition;

            // TODO: handle more than 2 Echoes? (hardly occures!)
            if (Echoes.Count > 1)
            {   // found 2 feet
                TwoFeet();
            }
            else if (Echoes.Count > 0)
            {   // found 1 foot
                OneFoot();
            }
            else
            {   // no feet found
                NoFoot();
            }

            FootFiltering();


            // update center position
            centerPosition = (leftFootPosition + rightFootPosition) / 2;
            centerPosition.y = 0;
            transform.position = centerPosition;
            //centerPosition += new Vector3(Orientation.x, 0, Orientation.y) * 0.3f;

            OrientationFiltering();
            FootAndHipHeight();
        }

        void IHeadReceiver.VectorData(float[] position, float[] rotation) {
            Vector2 headOrientation2D = new Vector2(rotation[0], rotation[2]).normalized;

            // get angle between movement vector and head direction vector
            if (Vector2.Angle(Orientation, headOrientation2D) > 90) {
                walksBackwards = true;
            }
            else {
                walksBackwards = false;
            }
            Debug.DrawRay(centerPosition, new Vector3(headOrientation2D.x, 0, headOrientation2D.y) * 1, Color.red);
        }

        void IDataReceiver.VectorData(float[] position, float[] rotation) {
            //throw new NotImplementedException();
        }
        #endregion
        #region debug drawing
        float lineDuration = 5;
        private void DrawTraces(Color l, Color r, Color c)
        {
            Debug.DrawLine(lastLeftFootPosition, leftFootPosition, l, lineDuration);
            Debug.DrawLine(lastRightFootPosition, rightFootPosition, r, lineDuration);
            Debug.DrawLine(lastCenterPosition, centerPosition, c, lineDuration);
            Debug.DrawRay(centerPosition, new Vector3(Orientation.x, 0, Orientation.y) * 0.5f, Color.magenta);
            Debug.DrawRay(centerPosition, new Vector3(fastOrientation.x, 0, fastOrientation.y) * 0.5f, Color.white);

            if (useLineRenderers)
            {
                leftFoot.AddFootTracePoint(leftFootPosition);
                rightFoot.AddFootTracePoint(rightFootPosition);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerPosition, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leftFootPosition, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightFootPosition, 0.1f);
        }
        #endregion
    }
}