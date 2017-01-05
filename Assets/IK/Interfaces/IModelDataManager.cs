using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularIK {
    public interface IModelDataManager {
        void Register(IModelTransform obj);
        void UnRegister(IModelTransform obj);

        // call this in game loop
        void UpdateModelData(float dt);
        void UpdateCallbacks();
    }
}