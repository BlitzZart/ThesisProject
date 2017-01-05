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

        public void Register(IModelTransform obj) {
            if (!modelTransrom.Contains(obj))
                modelTransrom.Add(obj);
        }

        public void UnRegister(IModelTransform obj) {
            if (modelTransrom.Contains(obj))
                modelTransrom.Remove(obj);
        }

        public void UpdateModelData(float dt) {

        }

        public void UpdateCallbacks() {

        }
    }
    #endregion
}