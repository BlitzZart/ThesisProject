using System;

namespace ModularIK {
    public class FootData : AComponentData {
        public FootData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}