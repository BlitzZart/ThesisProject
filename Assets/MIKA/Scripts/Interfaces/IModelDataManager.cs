using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIKA {
    public interface IModelDataManager {
        void SubscribeReceiver(IDataReceiver obj);
        void UnsubscribeReceiver(IDataReceiver obj);

        void AddProvider(AComponentData provider);
        void RemoveProvider(AComponentData provider);

        // call this in game loop
        void UpdateModelData();
        void UpdateCallbacks();
    }
}