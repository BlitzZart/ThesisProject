using Assets.Helper;
using ModularIK;
using System;
using UnityEngine;

class IKModelController : MonoBehaviour, IModelTransform, IModelLeftFootPosition, IModelRightFootPosition {
    IModelDataManager modelDataManager;


    private void Start() {
        modelDataManager.Register(this);
    }

    private void Update() {

    }

    private void OnDestroy() {
        modelDataManager.UnRegister(this);
    }

    public void ModelTransform(float[] position, float[] rotation) {
        
    }

    void ISingleIKVectorData.VectroData(float[] position) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    void IModelLeftFootPosition.VectroData(float[] position) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    void IModelRightFootPosition.VectroData(float[] position) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }
}
