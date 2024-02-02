using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{
    public override float getTargetAngle()
    {
        Vector3 direction = character.transform.position - target.transform.position;
        float targetAngle = Mathf.Atan2(-direction.x, direction.z) * Mathf.Rad2Deg;

        return targetAngle;
    }
}
