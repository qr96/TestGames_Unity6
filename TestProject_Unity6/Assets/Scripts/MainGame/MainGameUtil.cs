using UnityEngine;

public static class MainGameUtil
{
    static readonly float INV_SQRT2 = 0.707107f;
    static readonly Vector2[] OCTA_DIRETIONS =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(INV_SQRT2, INV_SQRT2),
        new Vector2(INV_SQRT2, -INV_SQRT2),
        new Vector2(-INV_SQRT2, -INV_SQRT2),
        new Vector2(-INV_SQRT2, INV_SQRT2)
    };

    public static Vector3 ToOcta(this Vector3 vector)
    {
        var vector2 = new Vector2(vector.x, vector.z).ToOcta();
        return new Vector3(vector2.x, vector.y, vector2.y);
    }

    public static Vector2 ToOcta(this Vector2 vector)
    {
        if (vector == Vector2.zero)
            return Vector2.zero;

        vector.Normalize();

        var closest = OCTA_DIRETIONS[0];
        var maxDot = Vector2.Dot(closest, vector);

        for (int i = 1; i < OCTA_DIRETIONS.Length; i++)
        {
            var dot = Vector2.Dot(vector, OCTA_DIRETIONS[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = OCTA_DIRETIONS[i];
            }
        }

        return closest;
    }
}
