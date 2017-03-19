using UnityEngine;

public class PlayerFootDings : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 1.1f)]
    public float speed, speed01, height;

    private Vector3 lastPosition;

    SimpleKalman kalman;

    public AnimationCurve curve;

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
    }

    void Update() {
        UseSimpleKalman();
    }
    #endregion

    private void UseSimpleKalman() {
        // get speed and clamp it to max speed
        speed = Mathf.Clamp((transform.position - lastPosition).magnitude / Time.deltaTime, 0, maxSpeed);
        // store last foot position
        lastPosition = transform.position;

        // scale speed to 0 - 1
        speed01 = speed / maxSpeed;
        // apply filter
        speed01 = (float)kalman.UseFilter(speed01);// CalcKalman(speed01);
        // take height from curve
        height = curve.Evaluate(speed01);
        // apply height
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}