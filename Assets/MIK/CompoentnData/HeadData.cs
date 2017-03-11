using System;

namespace MIKA {
    public class HeadData : AComponentData {
        public HeadData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}