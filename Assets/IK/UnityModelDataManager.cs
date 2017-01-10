using System;
using ModularIK;
using UnityEngine;
using System.Collections.Generic;


class UnityModelDataManager : MonoBehaviour {
    private UMDM umdm;
    private IKModelController IKModelController;

    #region unity callbacks
    private void Start() {
        umdm = new UMDM();
        IKModelController = new IKModelController(umdm);
    }

    private void Update() {
        umdm.UpdateModelData();
        umdm.UpdateCallbacks();
    }
    #endregion

    #region public
    public void Register(IDataReceiver obj) {
        umdm.Register(obj);
    }

    public void UnRegister(IDataReceiver obj) {
        umdm.UnRegister(obj);
    }
    #endregion

    #region private
    private class UMDM : IModelDataManager {
        private List<IDataReceiver> modelTransrom;
        private List<IDataReceiver> singleData;
        private ModelData modelData;

        public void Register(IDataReceiver obj) {
            if (!modelTransrom.Contains(obj))
                modelTransrom.Add(obj);
        }

        public void UnRegister(IDataReceiver obj) {
            if (modelTransrom.Contains(obj))
                modelTransrom.Remove(obj);
        }

        public void UpdateModelData() {
            modelData.Update();
        }

        public void UpdateCallbacks() {
            foreach (IDataReceiver item in modelTransrom) {
                item.VectorData(modelData.HipPosition, modelData.HipRotation);

                if (item is ILeftFootReceiver)
                    item.VectorData(modelData.LeftFootPosition, modelData.LeftFootRotation);
                else if (item is IRightFootReceiver)
                    item.VectorData(modelData.RightFootPosition, modelData.RightFootRotation);
            }
        }
    }
    #endregion
}