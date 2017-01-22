﻿using System;

namespace ModularIK {
    public class LeftFootData : AComponentData {
        public LeftFootData(Func<float[]> fposition, Func<float[]> fRotation) : base(fposition, fRotation) { }
    }
}