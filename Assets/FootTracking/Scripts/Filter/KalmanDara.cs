using UnityEngine;
using System.Collections;

public class KalmanDara : MonoBehaviour {
    public Transform mPoint;

    Matrix4x4 A, B, H, Q, R;


    Vector4 x, y, c = Vector2.zero;
    Vector4 m = Vector4.zero;

    Matrix4x4 P, S, K = Matrix4x4.zero;



    void Start() {
        A = InitMatrix(1, 0, 0.2f, 0,
                        0, 1, 0, 0.2f,
                        0, 0, 1, 0,
                        0, 0, 0, 1);


        B = InitMatrix(1, 0, 0, 0,
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1);

        H = InitMatrix(1, 0, 1, 0,
                        0, 1, 0, 1,
                        0, 0, 0, 0,
                        0, 0, 0, 0);

        Q = InitMatrix(0, 0, 0, 0,
                        0, 0, 0, 0,
                        0, 0, 0.1f, 0,
                        0, 0, 0, 0.1f);

        B = InitMatrix(0.1f, 0, 0, 0,
                        0, 0.1f, 0, 0,
                        0, 0, 0.1f, 0,
                        0, 0, 0, 0.1f);
    }

    void Update() {
        GetData();
        UpdateKalman();
    }


    private Matrix4x4 InitMatrix(float e01, float e02, float e03, float e04,
                                  float e11, float e12, float e13, float e14,
                                  float e21, float e22, float e23, float e24,
                                  float e31, float e32, float e33, float e34) {

        Matrix4x4 m = new Matrix4x4();

        m.SetRow(0, new Vector4(e01, e02, e03, e04));
        m.SetRow(1, new Vector4(e11, e12, e13, e14));
        m.SetRow(2, new Vector4(e21, e22, e23, e24));
        m.SetRow(3, new Vector4(e31, e32, e33, e34));

        return m;
    }

    private void UpdateKalman() {
        // prediction
        x = (A * x) + (B * c);
        P = (A * P * A.transpose);// + Q;

        // correction
        S = (H * P * H.transpose); // + R;
        K = (P * H.transpose * S.inverse);
        y = m - (H * x);
        x = x + (K * y);
        P = (/*Matrix4x4.identity - */(K * H)) * P;
    }

    private void GetData() {
        m = Camera.main.ScreenToViewportPoint(Input.mousePosition) + new Vector3(3,0,0);
        m.x = m.x + Random.Range(-0.5f, 0.5f);
        m.y = m.y + Random.Range(-0.5f, 0.5f);

        mPoint.position = new Vector2(m.x, m.y);
        print(m);
    } 




    //public double UseFilter(double value) {
    //    return KalmanUpdate(value);
    //}

    //void measurementUpdate() {
    //    K = (P + Q) / (P + Q + R);
    //    P = R * (P + Q) / (R + P + Q);
    //}

    //public double KalmanUpdate(double measurement) {
    //    measurementUpdate();

    //    double result = X + (measurement - X) * K;
    //    X = result;
    //    return result;
    //}

    public void PerfomKalmanTest() {
        int[] DATA = new int[16] { 0, 0, 0, 0, 1, 1, 2, 2, 2, 100, 10, 2, 3, 3, 1, 0 };

        //for (int i = 0; i < DATA.Length; i++) {
        //    Debug.Log(Mathf.Round((float)KalmanUpdate(DATA[i])) + ",");
        //}
    }
}