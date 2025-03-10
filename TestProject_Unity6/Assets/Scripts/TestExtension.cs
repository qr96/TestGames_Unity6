using UnityEngine;

public static class TestExtension
{
    public static Vector3 ToVector3(this float[] array)
    {
        return new Vector3(array[0], array[1], array[2]);
    }

    public static Quaternion ToEuler(this float[] array)
    {
        return Quaternion.Euler(array[0], array[1], array[2]);
    }

    public static float[] ToArray(this Vector3 vector3)
    {
        return new float[] { vector3.x, vector3.y, vector3.z };
    }
}
