using System;

namespace MIKA {
    public class LeftFootData : AComponentData {
        public LeftFootData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}