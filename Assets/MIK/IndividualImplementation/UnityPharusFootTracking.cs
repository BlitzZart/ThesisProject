using UnityEngine;

namespace ModularIK
{
    class UnityPharusFootTracking : ATrackingEntity
    {

        public PlayerFoot playerFootPrefab;
        public PlayerFoot leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure

        private LeftFootData leftFootData; //tempFootData; TODO: find a solution for temtFootData (Maybe not needed bacause the processing happens earlier)
        private RightFootData rightFootData;
        private HipData hipData;
        private Vector3 leftFootPosition, lastLeftFootPosition, rightFootPosition, lastRightFootPosition, tempFootPos;
        private Vector3 lastCenterPosition;
        private Vector3 centerPosition;

        private bool useLineRenderers = false;

        public bool applyFilterOnFeet = true;
        public bool correctCrossing = true;
        [Range(0.1f, 0.0000001f)]
        public double kalmanQ = 0.00005;
        [Range(0.1f, 0.00001f)]
        public double kalmanR = 0.01;

        SimpleKalman[] filters;

        private float _scaling = 0.01f;

        #region unity callbacks
        private void Start()
        {
            // init filter
            filters = new SimpleKalman[4];

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i] = new SimpleKalman();
                filters[i].Q = kalmanQ;
                filters[i].R = kalmanR;
            }

            InitFeet();
        }

        private void Update()
        {
            NewTracking();
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
        }
        #endregion

        #region public
        public override void SetPosition(Vector2 coords)
        {
            // transform tracking data. flip y and z axis and scale (1 unit = 1 meter)
            //transform.position = new Vector3(coords.x, 0, coords.y) * _scaling;
        }

        #endregion

        #region private

        private void InitFeet()
        {
            leftFootData = new LeftFootData(() => GetLeftFootPosition(), () => GetCenterRotation()/*GetLeftFootRotation()*/);
            rightFootData = new RightFootData(() => GetRightFootPosition(), () => GetCenterRotation()/*GetRightFootRotation()*/);
            hipData = new HipData(() => GetCenterPosition(), () => GetCenterRotation());

            UnityModelDataManager mdm = GetComponent<UnityModelDataManager>();
            mdm.AddProvider(leftFootData);
            mdm.AddProvider(rightFootData);
            mdm.AddProvider(hipData);
        }
        private float[] GetLeftFootPosition()
        {
            return new float[] { leftFootPosition.x, leftFootPosition.y, leftFootPosition.z };
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
            return new float[] { rightFootPosition.x, rightFootPosition.y, rightFootPosition.z };
        }
        // TODO: consider to provide a non-monobehavior solution which approximates the hip position based on foot data
        private float[] GetCenterPosition()
        {
            return new float[] { centerPosition.x, centerPosition.y, centerPosition.z };
        }

        // TODO: this is the smoothed orientation of pharus
        private float[] GetCenterRotation()
        {
            return new float[] { Orientation.x, 0, Orientation.y };
        }

        private void TrackFeet()
        {
            if (Echoes == null)
                return;

            lastLeftFootPosition = leftFootPosition;
            lastRightFootPosition = rightFootPosition;

            if (Echoes.Count > 1)
            { // found 2 feet
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[1].x, Echoes[1].y);
                // TODO: get rid of "/ _scaling" at foot height
                rightFoot.transform.position = new Vector3(pos.x, rightFoot.transform.position.y / _scaling, pos.y) * _scaling;
                pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                leftFoot.transform.position = new Vector3(pos.x, leftFoot.transform.position.y / _scaling, pos.y) * _scaling;

                // feet are crossed
                if (leftFoot.transform.localPosition.x > rightFoot.transform.localPosition.x)
                {
                    Vector3 temp = leftFoot.transform.position;
                    leftFoot.transform.position = rightFoot.transform.position;
                    rightFoot.transform.position = temp;
                }
            }
            else if (Echoes.Count > 0)
            {  // found 1 foot
                Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
                tempFoot.transform.position = new Vector3(pos.x, tempFoot.transform.position.y / _scaling, pos.y) * _scaling;
                if (tempFoot.transform.localPosition.x < 0)
                {
                    leftFoot.transform.position = tempFoot.transform.position;
                }
                else
                {
                    rightFoot.transform.position = tempFoot.transform.position;
                }
            }

            // set
            leftFootPosition = leftFoot.transform.position;
            rightFootPosition = rightFoot.transform.position;
        }

        private void FilterFeet()
        {
            foreach (SimpleKalman item in filters)
            {
                item.Q = kalmanQ;
                item.R = kalmanR;
            }

            leftFoot.transform.position = new Vector3((float)filters[0].UseFilter(leftFoot.transform.position.x), leftFoot.transform.position.y, (float)filters[1].UseFilter(leftFoot.transform.position.z));
            rightFoot.transform.position = new Vector3((float)filters[2].UseFilter(rightFoot.transform.position.x), rightFoot.transform.position.y, (float)filters[3].UseFilter(rightFoot.transform.position.z));

            leftFootPosition = leftFoot.transform.position;
            rightFootPosition = rightFoot.transform.position;
        }

        private void CalculateHip()
        {
            centerPosition = (rightFootPosition + leftFootPosition) / 2;
        }

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
        #endregion

        // check if a point is on the left side of the center line (aligned to orientation)
        private bool IsLeftOfCenter(Vector2 c)
        {
            Vector2 a = new Vector2(centerPosition.x, centerPosition.z);
            Vector2 b = a + Orientation;
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
        }

        private void ReinitializeAllFilters()
        {
            foreach (SimpleKalman item in filters)
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

        private void NewFilterFeet()
        {
            if (!applyFilterOnFeet)
                return;

            foreach (SimpleKalman item in filters)
            {
                item.Q = kalmanQ;
                item.R = kalmanR;
            }

            leftFootPosition = new Vector3((float)filters[0].UseFilter(leftFootPosition.x), leftFootPosition.y, (float)filters[1].UseFilter(leftFootPosition.z));
            rightFootPosition = new Vector3((float)filters[2].UseFilter(rightFootPosition.x), rightFootPosition.y, (float)filters[3].UseFilter(rightFootPosition.z));
        }

        private void NewTracking()
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
            {   // no foot found
                NoFoot();
            }

            NewFilterFeet();
            // update center position
            centerPosition = (leftFootPosition + rightFootPosition) / 2;
        }
        #region debug drawing
        float lineDuration = 5;
        private void DrawTraces(Color l, Color r, Color c)
        {
            Debug.DrawLine(lastLeftFootPosition, leftFootPosition, l, lineDuration);
            Debug.DrawLine(lastRightFootPosition, rightFootPosition, r, lineDuration);
            Debug.DrawLine(lastCenterPosition, centerPosition, c, lineDuration);
            Debug.DrawRay(centerPosition, new Vector3(Orientation.x, 0, Orientation.y) * 0.5f, Color.magenta);

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