using System;

namespace ModularIK {
    public class LeftHandData : AComponentData {
        public LeftHandData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}