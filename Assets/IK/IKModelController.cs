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
    private IModelDataManager modelDataManager;
    private Transform ikBody;
    IKControl ikControl;

    public IKModelController(IModelDataManager mdm, IKControl ik) {
        modelDataManager = mdm;
        modelDataManager.SubscribeReceiver(this);

        AssignIKModel(ik);
    }

    #region private
    private void AssignIKModel(IKControl ikControl) {
        this.ikControl = ikControl;

        //if (this.ikControl == null)
        //    return;

        //if (this.ikControl.rightFootObj || this.ikControl.leftFootObj)
        //    return;

        //ikBody = this.ikControl.transform;

        //ik.rightFootObj = rightFoot;
        //ik.leftFootObj = leftFoot;
    }

    // TODO: fix disposing - this is not a MonoBehaviour anymore
    private void OnDestroy() {
        modelDataManager.UnsubscribeReceiver(this);
    }
    #endregion

    // TODO: get rid of this implementation
    void ICenterReceiver.VectorData(float[] position, float[] rotation) {
    }

    void IDataReceiver.VectorData(float[] position, float[] rotation) {
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " | x " + position[0] + " y " + position[2]);
    }

    void ILeftFootReceiver.VectorData(float[] position, float[] rotation) {
        ikControl.leftFootPosition = new Vector3(position[0], position[1], position[2]);
        //Debug.Log("LeftFootReceiver " + position[0]);
    }

    void IRightFootReceiver.VectorData(float[] position, float[] rotation) {
        ikControl.rightFootPosition = new Vector3(position[0], position[1], position[2]);
        //Debug.Log("RightFootReceiver " + position[0]);
    }
}