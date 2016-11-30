using UnityEngine;
using System.Collections;

public class SpeedTest : MonoBehaviour {

    private Vector3 lastPos;
    public float velocity;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        velocity = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;
    }
}
