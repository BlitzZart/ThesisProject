using UnityEngine;
using UnscentedKalmanFilter;
using MathNet.Numerics.LinearAlgebra.Double;
using Example;
using System;
using MathNet.Numerics.LinearAlgebra;

public class PlayerFoot : MonoBehaviour {
    private float maxSpeed = 3;
    [Range(-0.1f, 1.1f)]
    public float speed, speed01, height;

    private Vector3 lastPosition;

    SimpleKalman kalman;

    // ------------------------------------------
    // ----------------- UKF --------------------
    UKF ukf = new UKF(1, 1);

    int n = 1; //number of state
    double q = 0.05; //std of process 
    double r = 0.3; //std of measurement
    int N = 100; //total dynamic steps

    Matrix<double> Q, R, x, P;
    FEquation f;
    HEquation h;
    // ------------------------------------------
    // ------------------------------------------

    public AnimationCurve curve;

    #region unity callbacks
    void Start() {
        kalman = new SimpleKalman();
        //InitUKF();
    }

    void Update() {
        //UseUKF();
        UseSimpleKalman();
    }
    #endregion

    private void InitUKF() {
        Q = Matrix.Build.Diagonal(n, n, q * q); //covariance of process
        R = Matrix.Build.Dense(1, 1, r * r); //covariance of measurement  
        f = new FEquation(); //nonlinear state equations
        h = new HEquation(); //measurement equation

        x = q * Matrix.Build.Random(1, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
        P = Matrix.Build.Diagonal(n, n, 1); //initial state covraiance

        //var xV = Matrix.Build.Dense(n, N, 0); //estmate
        //var zV = Matrix.Build.Dense(1, N, 0); //measurement

        //for (int k = 1; k < N; k++) {
        //    var z = Matrix.Build.Dense(1, 1, Math.Sin(k * 3.14 * 5 / 180)).Add(Matrix.Build.Random(1, 1).Multiply(r)); //measurments
        //    zV.SetSubMatrix(0, k, z);                                        //save measurment
        //    var x_and_P = ukf.Update(f, x, P, h, z, Q, R);                //ukf 
        //    x = x_and_P[0];
        //    P = x_and_P[1];
        //    xV.SetColumn(k, x.Column(0).ToArray());                          //save estimate
        //}
    }

    private void UseUKF() {
        // get speed and clap it to max speed
        speed = Mathf.Clamp((transform.position - lastPosition).magnitude / Time.deltaTime, 0, maxSpeed);
        // store last foot position
        lastPosition = transform.position;

        // scale speed to 0 - 1
        speed01 = speed / maxSpeed;
        // apply filter
        print("A" + speed01);
        speed01 = (float)UpdateUKF(speed01);
        print("B " + speed01);
        // take height from curve
        height = curve.Evaluate(speed01);

        // apply height
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    private double UpdateUKF(float value) {

        //Q = Matrix.Build.Diagonal(n, n, q * q); //covariance of process
        //R = Matrix.Build.Dense(1, 1, r * r); //covariance of measurement  
        f = new FEquation(); //nonlinear state equations
        h = new HEquation(); //measurement equation

        //x = q * Matrix.Build.Random(1, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
        //P = Matrix.Build.Diagonal(n, n, 1); //initial state covraiance

        //var xV = Matrix.Build.Dense(n, N, 0); //estmate
        //var zV = Matrix.Build.Dense(1, N, 0); //measurement

        Matrix<double> z = Matrix.Build.Dense(1, 1, value);
        Matrix<double>[] x_and_P = ukf.Update(f, x, P, h, z, Q, R); //ukf 
        x = x_and_P[0];
        P = x_and_P[1];

        return x[0, 0];
    }

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