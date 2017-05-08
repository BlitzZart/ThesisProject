using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA {
    [Serializable]
    public class TrackedData {
        public float[] pos, rot;
        public TrackedData(float[] pos, float[] rot) {
            this.pos = pos;
            this.rot = rot;
        }
    }
}