using System;

namespace ModularIK {
    public class HipData : AComponentData {
        public HipData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}