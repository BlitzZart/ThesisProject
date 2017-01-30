using UnityEngine;

public class PlayerFoot : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 1.1f)]
    public float speed, speed01, height;

    private Vector3 lastPosition;
    SimpleKalman kalman;

    public AnimationCurve curve;


    // ---- trace rendering ----
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private int numberOfLineEntries = 512;
    private int currentLineEntries;
    // -------------------------

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
        kalman.Q = 0.00005;
        kalman.R = 0.01;
    }

    void Update() {
        EstimateFootHeight();
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
    }
    public void AddFootTracePoint(Vector3 point)
    {
        if (lineRenderer == null)
            return;

        if (currentLineEntries < numberOfLineEntries)
            lineRenderer.SetVertexCount(currentLineEntries + 1);

        lineRenderer.SetPosition(currentLineEntries++ % numberOfLineEntries, point);
    }
    #endregion

    #region private
    private void EstimateFootHeight() {
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
    #endregion
}