using System;

namespace MIKA {
    public class HipData : AComponentData {
        public HipData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}