using System;

namespace MIKA {
    public class RightHandData : AComponentData {
        public RightHandData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}