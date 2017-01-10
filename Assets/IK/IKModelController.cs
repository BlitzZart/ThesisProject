using Assets.Helper;
using ModularIK;
using UnityEngine;

/// <summary>
/// The IKModelController controls the IK-Model 
/// and gets all transformation data from the ModelDataManager.
/// 
/// Set this up to fit the needs of the IK-Avatar.
/// Functionality is controlled by implemented interfaces (IDataReceiver).
/// </summary>
class IKModelController :  ICenterReceiver, ILeftFootReceiver, IRightFootReceiver {
    IModelDataManager modelDataManager;

    #region public
    public IKModelController(IModelDataManager mdm) {
        modelDataManager = mdm;
        modelDataManager.Register(this);
    }

    private void Update() {
    }

    // TODO: fix disposing - this is not a MonoBehaviour anymore
    private void OnDestroy() {
        modelDataManager.UnRegister(this);
    }
    #endregion

    // TODO: get rid of this implementation
    void ICenterReceiver.VectorData(float[] position, float[] rotation) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    void IDataReceiver.VectorData(float[] position, float[] rotation) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    void ILeftFootReceiver.VectorData(float[] position, float[] rotation) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    void IRightFootReceiver.VectorData(float[] position, float[] rotation) {
        ConsoleWriter.Instance.Write(this, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }
}