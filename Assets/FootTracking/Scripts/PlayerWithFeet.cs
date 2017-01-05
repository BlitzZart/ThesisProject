using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEditor;

public class PlayerWithFeet : ATrackingEntity {
    public bool useCorrectedCenter = true;
    public bool moveBodyInScript = false;

    public Transform playerFootPrefab, body, correctedCenter;
    public Material[] _feetMaterials;
    public Transform leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure

    private Transform ikBody;

    //add offset to adjust tracking area without changing the whole setup
    private Vector2 offset = new Vector2(0.0f, 0.0f);// offset in cm // TODO: also add to feet
    public float Height;

    private float _correctedCenterPosition = 0.65f;
    private float _headHeight = 4;
    private float _scaling = 0.01f;

    #region unity callbacks
    void Start() {
        leftFoot = Instantiate(playerFootPrefab);
        rightFoot = Instantiate(playerFootPrefab);
        tempFoot = Instantiate(playerFootPrefab);
        tempFoot.GetComponent<MeshRenderer>().enabled = false;

        leftFoot.parent = transform;
        rightFoot.parent = transform;
        tempFoot.parent = transform;

        leftFoot.GetComponent<MeshRenderer>().material = _feetMaterials[0];
        rightFoot.GetComponent<MeshRenderer>().material = _feetMaterials[1];

        AssignIKModel();
        AssignAIAgent(correctedCenter);
    }

    void Update() {
        TrackFeet();
        SetAvatarPosition();
        SetAvatarOrientation();
        EstimateCorrectedCenter();

        CheckEchos();
        //CalcFootVelocity();
        //ApplyFootHeight();
    }
    #endregion

    #region public

    public void SetUsedCenter(bool corrected) {
        if (corrected) {
            useCorrectedCenter = true;
            AssignAIAgent(correctedCenter);
        }
        else {
            useCorrectedCenter = false;
            AssignAIAgent(body);
        }
    }

    public override void SetPosition(Vector2 coords) {
        //this.transform.position = coords * _scaling;
        this.transform.position = new Vector3(coords.x + offset.x, 0, coords.y + offset.y) * _scaling;
    }
    #endregion

    #region private
    private void SetAvatarPosition() {
        if (ikBody == null || !moveBodyInScript)
            return;

        if (useCorrectedCenter)
            ikBody.position = correctedCenter.transform.position;
        else
            ikBody.position = transform.position;

        ikBody.rotation = Quaternion.LookRotation(new Vector3(Orientation.x, 0, Orientation.y));
    }

    private void SetAvatarOrientation() {
        Vector3 dir = new Vector3(Orientation.x, 0, Orientation.y);
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private void AssignIKModel() {
        IKControl ik = FindObjectOfType<IKControl>();
        if (ik == null)
            return;

        if (ik.rightFootObj || ik.leftFootObj)
            return;

        ikBody = ik.transform;

        ik.rightFootObj = rightFoot;
        ik.leftFootObj = leftFoot;
    }

    private void AssignAIAgent(Transform target) {
        AICharacterControl ai = FindObjectOfType<AICharacterControl>();

        if (ai == null || moveBodyInScript)
            return;

        ai.SetTarget(target);
    }

    private void TrackFeet() {
        if (Echoes == null)
            return;
        if (Echoes.Count > 1) { // found 2 feet
            Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[1].x, Echoes[1].y);
            rightFoot.position = new Vector3(pos.x, rightFoot.position.y, pos.y) * _scaling;
            pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
            leftFoot.position = new Vector3(pos.x, leftFoot.position.y, pos.y) * _scaling;

            // feet are crossed
            if (leftFoot.localPosition.x > rightFoot.localPosition.x) {
                Vector3 temp = leftFoot.position;
                leftFoot.position = rightFoot.position;
                rightFoot.position = temp;

            }
        }
        else if (Echoes.Count > 0) {  // found 1 foot
            Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
            tempFoot.position = new Vector3(pos.x, tempFoot.position.y, pos.y) * _scaling;
            if (tempFoot.localPosition.x < 0) {
                leftFoot.position = tempFoot.position;
            } else {
                rightFoot.position = tempFoot.position;
            }
        }
    }

    private void EstimateCorrectedCenter() {
        if (Speed > 0.25f) {
            correctedCenter.position = transform.position
                + new Vector3(Orientation.x, 0, Orientation.y) * _correctedCenterPosition;
        }
    }

    private void CheckEchos() {
        if (Echoes.Count < 1) {
            Debug.LogError("lost both foot echos");
            //print("lost both foot echos");
        }
        else if (Echoes.Count < 2) {
            Debug.LogError("lost one foot echo");
            //print("lost one foot echo");
        }
    }
    #endregion
}