using UnityEngine;

public class PlayerFoot : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 3.1f)]
    public float speed, speed01, height;

    private Vector3 footDirection, footVelocity;

    // TODO: Only for präsi
    public bool enableFootHeight = false;

    //private Vector3 lastPosition;
    SimpleKalman kalman;

    public AnimationCurve curve;
    public AnimationCurve rotationCurve;

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
        kalman.Q = 0.00005;
        kalman.R = 0.01;
    }
    #endregion

    #region public
    public Vector3 EstimateRotation(Vector3 position, Vector3 lastPosition)
    {
        Vector3 direction = (Vector3.SmoothDamp(footDirection, position - lastPosition, ref footVelocity, 0.66f) * rotationCurve.Evaluate(speed01)) * 0.5f;
        footDirection = direction;
        return footDirection.normalized;
    }
    public float EstimateHeight(Vector3 position, Vector3 lastPosition) {
        if (!enableFootHeight)
        {
            transform.position = position;
            return 0;
        }


        // TODO: Get rid of this hack!
        position.y = lastPosition.y = 0;
        // get speed and clamp it to max speed
        speed = Mathf.Clamp((position - lastPosition).magnitude / Time.fixedDeltaTime, 0, maxSpeed);
        // scale speed to 0 - 1
        speed01 = speed / maxSpeed;
        // apply filter
        speed01 = (float)kalman.UseFilter(speed01);// CalcKalman(speed01);
        // take height from curve
        height = curve.Evaluate(speed01);
        // apply height
        transform.position = new Vector3(position.x, height, position.z);

        return height;
    }
    #endregion
}