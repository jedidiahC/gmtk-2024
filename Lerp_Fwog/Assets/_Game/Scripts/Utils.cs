using UnityEngine;
public static class Utils {
    public static Vector2 QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        return new Vector2(
            Mathf.Pow(1 - t, 2) * start.x +
            (1 - t) * 2 * t * control.x +
            t * t * end.x,

            Mathf.Pow(1 - t, 2) * start.y +
            (1 - t) * 2 * t * control.y +
            t * t * end.y);
    }

    public static Vector2 CubicBezier(Vector2 start, Vector2 ctrlA, Vector2 ctrlB, Vector2 end, float t)
    {
        return new Vector2(
            Mathf.Pow(1 - t, 3) * start.x +
            Mathf.Pow(1 - t, 2) * 3 * t * ctrlA.x +
            (1 - t) * 3 * t * t * ctrlB.x +
            t * t * t * end.x,

            Mathf.Pow(1 - t, 3) * start.y +
            Mathf.Pow(1 - t, 2) * 3 * t * ctrlA.y +
            (1 - t) * 3 * t * t * ctrlB.y +
            t * t * t * end.y);
    }
}
