using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerWithFeet : ATrackingEntity {

    SimpleKalman kalman;

    public bool useCorrectedCenter = true;
    public bool moveBodyInScript = false;

    public Transform playerFootPrefab, body, correctedCenter;
    public Material[] _feetMaterials;
    public Transform leftFoot, rightFoot, tempFoot; // temp foot is used for food exchange when crossovers occure
    private Vector3 leftFootLast, rightFootLast;
    public float leftFootSpeed, rightFootSpeed;

    [Range(0.0f,100.0f)]
    public float leftFootHight, rightFootHight;
    private float maxFootHight = 100.0f;

    private Transform ikBody;

    //add offset to adjust tracking area without changing the whole setup
    private Vector2 offset = new Vector2(0.0f, 0.0f);// offset in cm // TODO: also add to feet
    public float Height;

    private float _correctedCenterPosition = 0.47f;
    private float _headHeight = 4;
    private float _scaling = 0.01f;

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
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
        CalcFootVelocity();
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

        //GetComponentInChildren<MeshRenderer>().enabled = false;

        if (ik.rightFootObj || ik.leftFootObj)
            return;

        ikBody = ik.transform;

        ik.rightFootObj = rightFoot;
        ik.leftFootObj = leftFoot;
    }

    public void AssignAIAgent(Transform target) {
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
            rightFoot.position = new Vector3(pos.x, rightFootHight, pos.y) * _scaling;
            pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
            leftFoot.position = new Vector3(pos.x, leftFootHight, pos.y) * _scaling;

            // feet are crossed
            if (leftFoot.localPosition.x > rightFoot.localPosition.x) {
                Vector3 temp = leftFoot.position;
                leftFoot.position = rightFoot.position;
                rightFoot.position = temp;
            }
        }
        else if (Echoes.Count > 0) {  // found 1 foot
            Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[0].x, Echoes[0].y);
            tempFoot.position = new Vector3(pos.x, leftFootHight, pos.y) * _scaling;
            if (tempFoot.localPosition.x < 0) {
                leftFoot.position = tempFoot.position;
            } else {
                rightFoot.position = tempFoot.position;
            }
        }
    }

    private void CalcFootVelocity() {
        leftFootSpeed = (leftFoot.position - leftFootLast).magnitude / Time.deltaTime;
        leftFootLast = leftFoot.position;
        //print("LF " + leftFootSpeed);
        rightFootSpeed = (rightFoot.position - rightFootLast).magnitude / Time.deltaTime;
        rightFootLast = rightFoot.position;
    }

    private void ApplyFootHeight() {
        leftFootHight = CalcLerp(leftFootHight, leftFootSpeed);// CalcKalman(leftFootSpeed);
        rightFootHight = CalcLerp(rightFootHight, rightFootSpeed);
    }

    private float CalcKalman(float footSpeed) {
        return (float)kalman.UseFilter(Mathf.Clamp(footSpeed, 0, maxFootHight));
    }

    private float CalcLerp(float footHeight, float footSpeed) {
        return Mathf.Lerp(footHeight, Mathf.Clamp(footSpeed, 0, maxFootHight), 1000 * Time.deltaTime);
    }

    //float oldSpeed = 0;
    private void EstimateCorrectedCenter() {
        //float smoothSpeed = Mathf.Lerp(oldSpeed, Speed, 0.00000001f);

        if (Speed > 0.25f) {
            correctedCenter.position = transform.position
                + new Vector3(Orientation.x, 0, Orientation.y) * _correctedCenterPosition;
        }
        //oldSpeed = Speed;
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