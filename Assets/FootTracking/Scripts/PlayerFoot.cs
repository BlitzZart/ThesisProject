using UnityEngine;
using System.Collections;

public class PlayerFoot : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 1.1f)]
    public float speed, speed01, height;

    private Vector3 lastPosition;

    SimpleKalman kalman;

    public AnimationCurve curve;

    void Start() {
        kalman = new SimpleKalman();
    }

    void Update() {
        // get speed and clap it to max speed
        speed = Mathf.Clamp((transform.position - lastPosition).magnitude / Time.deltaTime, 0, maxSpeed);
        // store last foot position
        lastPosition = transform.position;

        // scale speed to 0 - 1
        speed01 = speed / maxSpeed;
        // apply filter
        speed01 = CalcKalman(speed01);
        // take height from curve
        height = curve.Evaluate(speed01);

        // apply height
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    private float CalcKalman(float value) {
        return (float)kalman.UseFilter(value);
    }
}