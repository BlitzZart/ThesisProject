using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularIK {
    public interface IModelDataManager {
        void Register(IDataReceiver obj);
        void UnRegister(IDataReceiver obj);

        // call this in game loop
        void UpdateModelData();
        void UpdateCallbacks();
    }
}