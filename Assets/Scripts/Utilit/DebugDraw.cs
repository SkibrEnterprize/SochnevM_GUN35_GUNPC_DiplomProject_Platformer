using UnityEngine;

public static class DebugDraw
{
    public static void Sphere(Vector3 center, float radius, Color color, float duration = 0.5f)
    {
        float angleStep = 45f; // Чем меньше шаг, тем круглее сфера
        for (float a = 0; a < 360; a += angleStep)
        {
            float r1 = a * Mathf.Deg2Rad;
            float r2 = (a + angleStep) * Mathf.Deg2Rad;

            // Рисуем три кольца: горизонтальное, вертикальное и боковое
            Vector3 s1 = new Vector3(Mathf.Cos(r1), 0, Mathf.Sin(r1)) * radius;
            Vector3 e1 = new Vector3(Mathf.Cos(r2), 0, Mathf.Sin(r2)) * radius;
            Debug.DrawLine(center + s1, center + e1, color, duration);

            Vector3 s2 = new Vector3(0, Mathf.Cos(r1), Mathf.Sin(r1)) * radius;
            Vector3 e2 = new Vector3(0, Mathf.Cos(r2), Mathf.Sin(r2)) * radius;
            Debug.DrawLine(center + s2, center + e2, color, duration);

            Vector3 s3 = new Vector3(Mathf.Cos(r1), Mathf.Sin(r1), 0) * radius;
            Vector3 e3 = new Vector3(Mathf.Cos(r2), Mathf.Sin(r2), 0) * radius;
            Debug.DrawLine(center + s3, center + e3, color, duration);
        }
    }
}