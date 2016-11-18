using UnityEngine;
using System.Collections;
using System;

public class PlayerWithFeet : ATrackingEntity {

    public bool useCorrectedCenter = true;

    public Transform playerFootPrefab, body, correctedCenter;
    public Material[] _feetMaterials;
    private Transform[] _feet = new Transform[2];
    private Transform ikBody;

    //add offset to adjust tracking area without changing the whole setup
    private Vector2 offset = new Vector2(0.0f, 0.0f);// offset in cm // TODO: also add to feet
    public float Height;

    private float _correctedCenterPosition = 0.5f;
    private float _headHeight = 4;
    private float _scaling = 0.01f;

    #region unity callbacks
    void Start() {
        _feet[0] = Instantiate(playerFootPrefab);
        _feet[1] = Instantiate(playerFootPrefab);

        _feet[0].parent = transform;
        _feet[1].parent = transform;

        _feet[0].GetComponent<MeshRenderer>().material = _feetMaterials[0];
        _feet[1].GetComponent<MeshRenderer>().material = _feetMaterials[1];

        AssignIKModel();
    }

    void Update() {
        TrackFeet();
        SetAvatarPosition();
        EstimateHead();

        CheckEchos();
    }
    #endregion

    #region public
    public override void SetPosition(Vector2 coords) {
        //this.transform.position = coords * _scaling;
        this.transform.position = new Vector3(coords.x + offset.x, 0, coords.y + offset.y) * _scaling;
    }
    #endregion

    #region private
    private void SetAvatarPosition() {
        if (ikBody == null)
            return;

        if (useCorrectedCenter)
            ikBody.position = correctedCenter.transform.position;
        else
            ikBody.position = transform.position;

        ikBody.rotation = Quaternion.LookRotation(new Vector3(Orientation.x, 0, Orientation.y));
    }

    private void AssignIKModel() {
        IKControl ik = FindObjectOfType<IKControl>();
        if (ik == null)
            return;

        GetComponentInChildren<MeshRenderer>().enabled = false;

        if (ik.rightFootObj || ik.leftFootObj)
            return;

        ikBody = ik.transform;

        ik.rightFootObj = _feet[0];
        ik.leftFootObj = _feet[1];
    }

    private void TrackFeet() {
        if (Echoes == null)
            return;
        for (int i = 0; i < Echoes.Count; i++) {
            Vector3 pos = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[i].x, Echoes[i].y);
            _feet[i].position = new Vector3(pos.x, 0, pos.y) * _scaling;
        }
    }

    //float oldSpeed = 0;
    private void EstimateHead() {
        //float smoothSpeed = Mathf.Lerp(oldSpeed, Speed, 0.00000001f);

        if (Speed > 0.1f) {
            correctedCenter.position = transform.position
                + new Vector3(Orientation.x, 0, Orientation.y) * _correctedCenterPosition;
        }
        //oldSpeed = Speed;
    }

    private void CheckEchos() {
        if (Echoes.Count < 1) {
            print("lost both foot echos");
        }
        else if (Echoes.Count < 2) {
            print("lost one foot echo");
        }
    }
    #endregion
}