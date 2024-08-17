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

    public static Vector3 QuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return new Vector3(
            Mathf.Pow(1 - t, 2) * start.x +
            (1 - t) * 2 * t * control.x +
            t * t * end.x,

            Mathf.Pow(1 - t, 2) * start.y +
            (1 - t) * 2 * t * control.y +
            t * t * end.y,
            
            Mathf.Pow(1 - t, 2) * start.z +
            (1 - t) * 2 * t * control.z +
            t * t * end.z);
    }

    public static Vector3 CubicBezier(Vector3 start, Vector3 ctrlA, Vector3 ctrlB, Vector3 end, float t)
    {
        return new Vector3(
            Mathf.Pow(1 - t, 3) * start.x +
            Mathf.Pow(1 - t, 2) * 3 * t * ctrlA.x +
            (1 - t) * 3 * t * t * ctrlB.x +
            t * t * t * end.x,

            Mathf.Pow(1 - t, 3) * start.y +
            Mathf.Pow(1 - t, 2) * 3 * t * ctrlA.y +
            (1 - t) * 3 * t * t * ctrlB.y +
            t * t * t * end.y,

            Mathf.Pow(1 - t, 3) * start.z +
            Mathf.Pow(1 - t, 2) * 3 * t * ctrlA.z +
            (1 - t) * 3 * t * t * ctrlB.z +
            t * t * t * end.z);
    }

    public static void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        // Calculate the required local scale to achieve the target global scale
        Vector3 parentScale = transform.parent ? transform.parent.lossyScale : Vector3.one;
        transform.localScale = new Vector3(
            globalScale.x / parentScale.x,
            globalScale.y / parentScale.y,
            globalScale.z / parentScale.z
        );
    }
}
