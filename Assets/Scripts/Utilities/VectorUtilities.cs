using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtilities {

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float degreesToRotate)
    {
        Quaternion rotationAngles = Quaternion.Euler(0.0f, degreesToRotate, 0.0f);

        Vector3 direction = point - pivot;
        direction = rotationAngles * direction;
        return direction + pivot;
    }
}
