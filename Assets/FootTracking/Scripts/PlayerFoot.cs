using UnityEngine;

public class PlayerFoot : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 3.1f)]
    public float speed, speed01, height;

    private Vector3 footDirection;

    // TODO: Only for präsi
    public bool enableFootHeight = false;

    //private Vector3 lastPosition;
    SimpleKalman kalman;

    public AnimationCurve curve;
    public AnimationCurve rotationCurve;


    // ---- trace rendering ----
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private int numberOfLineEntries = 128;
    private int currentLineEntries;
    // -------------------------

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
        kalman.Q = 0.00005;
        kalman.R = 0.01;
    }
    #endregion

    #region public
    public void InitLineRenderer(Color color)
    {
        currentLineEntries = 0;

        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.SetVertexCount(numberOfLineEntries);
        lineRenderer.SetColors(color, color);

        lineRenderer.material = lineMaterial;
        lineRenderer.SetWidth(0.01f, 0.01f);
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.enabled = false;
    }
    public void AddFootTracePoint(Vector3 point)
    {
        if (lineRenderer == null)
            return;

        if (currentLineEntries < numberOfLineEntries)
            lineRenderer.SetVertexCount(currentLineEntries + 1);

        lineRenderer.SetPosition(currentLineEntries++ % numberOfLineEntries, point);
    }

    Vector3 footVelocity;
    public Vector3 EstimateRotation(Vector3 position, Vector3 lastPosition)
    {
        Vector3 direction = (Vector3.SmoothDamp(footDirection, position - lastPosition, ref footVelocity, 0.66f) * rotationCurve.Evaluate(speed01)) * 0.5f;
        footDirection = direction;
        return footDirection;
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