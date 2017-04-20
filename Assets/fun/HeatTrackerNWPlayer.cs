using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatTrackerNWPlayer : NetworkBehaviour {

    public GameObject tracker;
    public OVRCameraRig ovrRig;

	// Use this for initialization
	void Start () {
        if (isServer)
            StartCoroutine(GetTracker());
        else
            ovrRig = FindObjectOfType<OVRCameraRig>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer) {
            if (tracker == null)
                return;

            transform.position = tracker.transform.position;
            transform.rotation = tracker.transform.rotation;
        } else {
            ovrRig.transform.position = transform.position;
            ovrRig.transform.rotation = transform.rotation;
        }
    }

    private IEnumerator GetTracker() {
        while (tracker == null) {
            yield return new WaitForSeconds(0.1f);

            tracker = GameObject.FindGameObjectWithTag("HeadTracker");
        }
    }
}
