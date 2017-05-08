using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA {
    [Serializable]
    public class UnityTrackedData : TrackedData {
        public UnityTrackedData(Vector3 pos, Vector3 rot)
            : base(new float[] { pos.x, pos.y, pos.z }, new float[] { rot.x, rot.y, rot.z }) { }
    }
}