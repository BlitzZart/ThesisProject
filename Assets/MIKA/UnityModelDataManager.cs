using System;
using MIKA;
using UnityEngine;
using System.Collections.Generic;

class UnityModelDataManager : MonoBehaviour {
    public int playerAssigned = 0;
    private ModelDataManager mdm;
    private IKModelController IKModelController;

    #region unity callbacks
    private void Start() {
        mdm = new ModelDataManager();  

        IKModelController = new IKModelController(mdm, FindObjectOfType<IKControl>());
    }

    private void Update() {
        mdm.UpdateModelData();
        mdm.UpdateCallbacks();
    }
    #endregion

    #region public
    public void SubscribeReceiver(IDataReceiver obj) {
        mdm.SubscribeReceiver(obj);
    }

    public void UnsubscribeReseiver(IDataReceiver obj) {
        mdm.UnsubscribeReceiver(obj);
    }
    public void AddProvider(AComponentData provider) {
        mdm.AddProvider(provider);
    }
    public void RemoveProvider(AComponentData provider) {
        mdm.RemoveProvider(provider);
    }

    #endregion

    #region private
    private class ModelDataManager : IModelDataManager {
        private List<IDataReceiver> modelTransrom;
        private List<IDataReceiver> singleData;
        private ModelData modelData;

        public ModelDataManager() {
            modelData = new ModelData();
            modelTransrom = new List<IDataReceiver>();
        }

        public void SubscribeReceiver(IDataReceiver obj) {
            if (!modelTransrom.Contains(obj))
                modelTransrom.Add(obj);
        }

        public void UnsubscribeReceiver(IDataReceiver obj) {
            if (modelTransrom.Contains(obj))
                modelTransrom.Remove(obj);
        }

        public void UpdateModelData() {
            modelData.Update();
        }

        public void UpdateCallbacks() {
            foreach (IDataReceiver item in modelTransrom) {
                //item.VectorData(modelData.HipPosition, modelData.HipRotation);

                if (item is ILeftFootReceiver) {
                    (item as ILeftFootReceiver).VectorData(modelData.LeftFootPosition, modelData.LeftFootRotation);
                }
                if (item is IRightFootReceiver) {
                    (item as IRightFootReceiver).VectorData(modelData.RightFootPosition, modelData.RightFootRotation);
                }
                // TODO: Center = hip -> make consistency (names)
                if (item is ICenterReceiver)
                {
                    (item as ICenterReceiver).VectorData(modelData.HipPosition, modelData.HipRotation);
                }

                if (item is ILeftHandReceiver) {
                    (item as ILeftHandReceiver).VectorData(modelData.LeftHandPosition, modelData.LeftHandRotation);
                }
                if (item is IRightHandReceiver) {
                    (item as IRightHandReceiver).VectorData(modelData.RightHandPosition, modelData.RightHandRotation);
                }

                if (item is IHeadReceiver) {
                    (item as IHeadReceiver).VectorData(modelData.HeadPosition, modelData.HeadRotation);
                }
            }
        }

        public void AddProvider(AComponentData provider) {
            modelData.AddProvider(provider);
        }
        public void RemoveProvider(AComponentData provider) {
            modelData.RemoveProvider(provider);
        }
    }
    #endregion
}