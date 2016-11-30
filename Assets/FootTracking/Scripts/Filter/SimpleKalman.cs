using UnityEngine;
using System.Collections;

public class SimpleKalman {
    public double Q = 0.000001;
    public double R = 0.01;
    public double P = 1, X = 0, K;



    public double UseFilter(double value) {

        return KalmanUpdate(value);
    }

    void measurementUpdate() {
        K = (P + Q) / (P + Q + R);
        P = R * (P + Q) / (R + P + Q);
    }

    public double KalmanUpdate(double measurement) {
        measurementUpdate();

        double result = X + (measurement - X) * K;
        X = result;
        return result;
    }

    void PerfomKalmanTest() {
        int[] DATA = new int[16] { 0, 0, 0, 0, 1, 1, 2, 2, 2, 100, 10, 2, 3, 3, 1, 0 };

        //for (int i = 0; i < DATA.Length; i++) {
        //    print(Mathf.Round((float)KalmanUpdate(DATA[i])) + ",");
        //}
    }
}
