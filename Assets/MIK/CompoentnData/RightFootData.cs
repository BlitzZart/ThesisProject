using System;

namespace MIKA {
    public class RightFootData : AComponentData {
        public RightFootData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}