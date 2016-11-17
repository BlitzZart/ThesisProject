using UnityEngine;
using System.Collections;
using System;

public class PlayerWithFeet : ATrackingEntity {
    public Transform playerFootPrefab, body, head;

    public Material[] _feetMaterials;

    private Transform[] _feet = new Transform[2];

    
    
    private float _headOffsetDistance = 120;
    private float _headOffsetLerpSpeed = 80;
    private float _headHeight = 200;


    #region unity callbacks
    void Start() {
        _feet[0] = Instantiate(playerFootPrefab);
        _feet[1] = Instantiate(playerFootPrefab);

        _feet[0].parent = transform;
        _feet[1].parent = transform;

        _feet[0].GetComponent<MeshRenderer>().material = _feetMaterials[0];
        _feet[1].GetComponent<MeshRenderer>().material = _feetMaterials[1];
    }

    void Update() {
        TrackFeet();
        //BodyOffset();
        EstimateHead();

        CheckEchos();




    }
    #endregion

    #region private
    private void EstimateHead() {
        if (Speed > 0.0f) {
            head.position = transform.position
                + new Vector3(Orientation.x, Orientation.y, 0) * _headOffsetDistance * Speed
                + Vector3.back * _headHeight;
        }
        //if (Speed > 0.1f) {
        //    head.position = Vector3.Lerp(transform.position, transform.position
        //        + new Vector3(Orientation.x, Orientation.y, 0) * _headOffsetDistance * Speed
        //        + Vector3.back * _headHeight, _headOffsetLerpSpeed * Time.deltaTime);
        //}

    }

    private void TrackFeet() {
        if (Echoes == null)
            return;

        for (int i = 0; i < Echoes.Count; i++) {
            _feet[i].position = UnityTracking.TrackingAdapter.GetScreenPositionFromRelativePosition(Echoes[i].x, Echoes[i].y);
        }
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