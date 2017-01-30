using UnityEngine;
public static class VectorHelpers
{
    public static Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    public static Vector2 RotateVector2(Vector2 aPoint, float aDegree)
    {
        float rad = aDegree * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);

        return new Vector2(aPoint.x * c - aPoint.y * s, aPoint.y * c + aPoint.x * s);
    }

    public static void Swap(ref Vector2 a, ref Vector2 b)
    {
        Vector2 temp = a;
        a = b;
        b = temp;
    }
    public static void Swap(ref Vector3 a, ref Vector3 b)
    {
        Vector3 temp = a;
        a = b;
        b = temp;
    }
}