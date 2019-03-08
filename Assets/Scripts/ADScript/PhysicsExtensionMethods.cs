using UnityEngine;

static class PhysicsExtensionMethods
{
    public static bool IsInSameDirection(this Vector3 vector, Vector3 otherVector)
    {
        return Vector3.Dot(vector, otherVector) > 0;
    }

    public static bool IsInOppositeDirection(this Vector3 vector, Vector3 otherVector)
    {
        return Vector3.Dot(vector, otherVector) < 0;
    }
}