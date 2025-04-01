using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawCone(Vector3 point, Vector3 direction, float degreeHalfAngle, float length, int resolution = 12) => DrawCone(point, direction, Vector3.up, degreeHalfAngle, length, resolution);

    public static void DrawCone(Vector3 point, Vector3 direction, Vector3 up, float degreeHalfAngle, float length, int resolution = 12)
    {
        if (direction == Vector3.zero)
        {
            Debug.Log($"{nameof(direction)} is zero");
            return;
        }
        if (up == Vector3.zero)
        {
            Debug.Log($"{nameof(up)} is zero");
            return;
        }
        if (degreeHalfAngle < 0)
        {
            Debug.Log($"{nameof(degreeHalfAngle)} < 0");
            return;
        }
        if (length <= 0)
        {
            Debug.Log($"{nameof(length)} <= 0");
            return;
        }
        if (resolution < 3)
        {
            Debug.Log($"{nameof(resolution)} can't be less than 3");
            return;
        }

        direction.Normalize();

        float radius = length * Mathf.Tan(degreeHalfAngle * Mathf.Deg2Rad);
        Vector3 right = Vector3.Cross(up, direction).normalized;
        Vector3 endPoint = radius * right + length * direction.normalized;

        float angleStep = 360f / resolution;
        for (int i = 0; i < resolution; i++)
        {
            Vector3 nextEndPoint = Quaternion.AngleAxis(-angleStep, direction) * endPoint;
            Gizmos.DrawLine(point, point + nextEndPoint);
            Gizmos.DrawLine(point + endPoint, point + nextEndPoint);

            endPoint = nextEndPoint;
        }
    }
}