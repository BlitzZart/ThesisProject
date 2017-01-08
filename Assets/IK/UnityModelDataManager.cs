using System;
using ModularIK;
using UnityEngine;
using System.Collections.Generic;

class UnityModelDataManager : MonoBehaviour {
    private UMDM umdm;

    #region unity callbacks
    private void Start() {
        umdm = new UMDM();
    }

    private void Update() {
        umdm.UpdateModelData(Time.deltaTime);
        umdm.UpdateCallbacks();
    }
    #endregion

    #region public
    public void Register(IModelTransform obj) {
        umdm.Register(obj);
    }

    public void UnRegister(IModelTransform obj) {
        umdm.UnRegister(obj);
    }
    #endregion

    #region private




    private class UMDM : IModelDataManager {
        private List<IModelTransform> modelTransrom;
        private List<ISingleIKVectorData> singleData;
        private ModelData modelData;

        public void Register(IModelTransform obj) {
            if (!modelTransrom.Contains(obj))
                modelTransrom.Add(obj);

            if (obj is ISingleIKVectorData) {
                if (!singleData.Contains(obj as ISingleIKVectorData)) {
                    singleData.Add((ISingleIKVectorData)obj);
                }
            }
        }

        public void UnRegister(IModelTransform obj) {
            if (modelTransrom.Contains(obj))
                modelTransrom.Remove(obj);

            if (obj is ISingleIKVectorData) {
                if (!singleData.Contains(obj as ISingleIKVectorData)) {
                    singleData.Add((ISingleIKVectorData)obj);
                }
            }
        }

        public void UpdateModelData(float dt) {
        }

        public void UpdateCallbacks() {
            foreach (IModelTransform item in modelTransrom) {
                item.ModelTransform(modelData.HipPosition, modelData.HipRotation);
            }
            foreach(ISingleIKVectorData item in singleData) {
                if (item is IModelLeftFootPosition)
                    item.VectroData(modelData.LeftFootPosition);
                else if (item is IModelRightFootPosition)
                    item.VectroData(modelData.RightFootPosition);
            }



        }
    }
    #endregion
}