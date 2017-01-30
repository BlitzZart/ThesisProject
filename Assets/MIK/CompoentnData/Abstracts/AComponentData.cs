using System;

namespace ModularIK {
    public abstract class AComponentData {
        protected Func<float[]> position, rotation;

        private bool usePosition, useRotation;
        public bool UsePosition
        {
            get
            {
                return usePosition;
            }

            set
            {
                usePosition = value;
            }
        }
        public bool UseRotation
        {
            get
            {
                return useRotation;
            }

            set
            {
                useRotation = value;
            }
        }

        public AComponentData(Func<float[]> fPosition, Func<float[]> fRotaton) {
            if (fPosition != null) {
                usePosition = true;
                position = fPosition;
            }

            if (fRotaton != null) {
                useRotation = true;
                rotation = fRotaton;
            }
        }

        public float[] Position() {
            return position();
        }
        public float[] Rotation() {
            if (rotation == null) // TODO: handle unused data properly
                return null;

            return rotation();
        }
    }
}