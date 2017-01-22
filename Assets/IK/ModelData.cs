using System;
using System.Collections.Generic;

namespace ModularIK {
    /// <summary>
    /// ModelData just provides all available and necessary data.
    /// Various DataProvider deliver this data.
    /// </summary>
    class ModelData {
        private List<AComponentData> providers = new List<AComponentData>();
        public List<AComponentData> Providers
        {
            get
            {
                return providers;
            }
        }

        private LeftFootData leftFoot;
        private RightFootData rightFoot;
        public LeftFootData LeftFoot
        {
            get
            {
                return leftFoot;
            }
        }
        public RightFootData RightFoot
        {
            get
            {
                return rightFoot;
            }
        }

        private float[] hipPosition = new float[3];
        private float[] hipRotation = new float[3];
        private float[] headPosition = new float[3];
        private float[] headRotation = new float[3];
        private float[] leftFootPosition = new float[3];
        private float[] leftFootRotation = new float[3];
        private float[] rightFootPosition = new float[3];
        private float[] rightFootRotation = new float[3];

        public float[] HipPosition
        {
            get
            {
                return hipPosition;
            }
        }
        public float[] HipRotation
        {
            get
            {
                return hipRotation;
            }
        }
        public float[] HeadPosition
        {
            get
            {
                return headPosition;
            }
        }
        public float[] HeadRotation
        {
            get
            {
                return headRotation;
            }
        }
        public float[] LeftFootPosition
        {
            get
            {
                return leftFootPosition;
            }
        }
        public float[] LeftFootRotation
        {
            get
            {
                return leftFootRotation;
            }
        }
        public float[] RightFootPosition
        {
            get
            {
                return rightFootPosition;
            }
        }
        public float[] RightFootRotation
        {
            get
            {
                return rightFootRotation;
            }
        }

        #region public
        public void AddProvider(AComponentData provider) {
            providers.Add(provider);
            if (provider is LeftFootData)
                leftFoot = (LeftFootData)provider;
            else if (provider is RightFootData)
                rightFoot = (RightFootData)provider;
        }
        public void RemoveProvider(AComponentData provider) {
            providers.Remove(provider);
            if (provider is LeftFootData)
                leftFoot = null;
            else if (provider is RightFootData)
                rightFoot = null;
        }

        public void Update() {
            foreach (AComponentData item in providers) {
                if (item is LeftFootData || item is RightFootData)
                    HandleFeet(item);
            }
        }
        #endregion

        #region private
        #region feet
        private void ProcessFeet() {

        }
        private void HandleFeet(AComponentData foot) {
            ProcessFeet();
            if (foot == leftFoot) {
                leftFootPosition = foot.Position();
                leftFootRotation = foot.Rotation();
            }
            else {
                rightFootPosition = foot.Position();
                rightFootRotation = foot.Rotation();
            }
        }

        #endregion

        #endregion
    }
}