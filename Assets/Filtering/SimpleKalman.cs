using UnityEngine;

public class SimpleKalman {
    public double Q = 0.00005;
    public double R = 0.01;
    public double P = 1, X = 0, K;

    public void Reinitialize()
    {
        P = 1;
        X = 0;
        K = 0;
    }

    public double UseFilter(double value) {
        return KalmanUpdate(value);
    }

    public double KalmanUpdate(double measurement) {
        MeasurementUpdate();

        double result = X + (measurement - X) * K;
        X = result;
        return result;
    }

    private void MeasurementUpdate() {
        K = (P + Q) / (P + Q + R);
        P = R * (P + Q) / (R + P + Q);
    }
}

//public void PerfomKalmanTest() {
//    int[] DATA = new int[16] { 0, 0, 0, 0, 1, 1, 2, 2, 2, 100, 10, 2, 3, 3, 1, 0 };

//    for (int i = 0; i < DATA.Length; i++) {
//        Debug.Log(Mathf.Round((float)KalmanUpdate(DATA[i])) + ",");
//    }
//}