using UnityEngine;
using System.Collections;

public class VisibilityManager : MonoBehaviour {
    public MeshRenderer body, correctedBody, leftFoot, rightFoot;
    public SkinnedMeshRenderer model;

    PlayerWithFeet player;

    void Start() {
        Init();

    }

    private void Init() {
        if (player == null)
            player = FindObjectOfType<PlayerWithFeet>();
        if (player == null)
            return;

        body = player.body.GetComponent<MeshRenderer>();
        correctedBody = player.correctedCenter.GetComponent<MeshRenderer>();
        rightFoot = player.rightFoot.GetComponent<MeshRenderer>();
        leftFoot = player.leftFoot.GetComponent<MeshRenderer>();

    }

    public void ShowBody() {
        Init();
        body.enabled = !body.enabled;
    }
    public void ShowCorrectedBody() {
        Init();
        correctedBody.enabled = !correctedBody.enabled;
    }
    public void ShowFeet() {
        Init();
        leftFoot.enabled = rightFoot.enabled = !leftFoot.enabled;
    }
    public void ShowModel() {
        Init();
        model.enabled = !model.enabled;
    }

}
