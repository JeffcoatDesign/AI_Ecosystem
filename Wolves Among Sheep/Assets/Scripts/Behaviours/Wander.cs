using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Wander : SteeringBehavior
{
    float wanderOffset = 1f;
    float wanderRadius = 5f;

    float wanderRate = 5f;

    float wanderOrientation;

    float maxAcceleration = 1f;

    Face face = new();

    public override SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        wanderOrientation += RandomBinomial() * wanderRate * Mathf.Rad2Deg;

        float targetOrientation = wanderOrientation + character.transform.eulerAngles.y;

        Vector3 targetPos = character.transform.position + (wanderOffset * Vector3.one) + character.transform.forward;

        targetPos += wanderRadius * targetOrientation * Vector3.one;

        result.angular = Face(targetPos).angular;

        result.linear = maxAcceleration * character.transform.forward;

        return result;
    }

    private float RandomBinomial()
    {
        return Random.value - Random.value;
    }
    public float getTargetAngle(Vector3 target)
    {
        Vector3 direction = character.transform.position - target;
        float targetAngle = Mathf.Atan2(-direction.x, direction.z) * Mathf.Rad2Deg;

        return targetAngle;
    }

    float maxAngularAcceleration = 100f; // 5
    float maxRotation = 45f; // maxAngularVelocity

    // the radius for arriving at the target
    //float targetRadius = 1f;

    // the radius for beginning to slow down
    float slowRadius = 10f;

    // the time over which to achieve target speed
    float timeToTarget = 0.1f;

    public SteeringOutput Face(Vector3 target)
    {
        SteeringOutput result = new SteeringOutput();

        // get the naive direction to the target
        //float rotation = Mathf.DeltaAngle(character.transform.eulerAngles.y, target.transform.eulerAngles.y);
        float rotation = Mathf.DeltaAngle(character.transform.eulerAngles.y, getTargetAngle(target));
        float rotationSize = Mathf.Abs(rotation);


        // if we are outside the slow radius, then use maximum rotation
        float targetRotation = 0.0f;
        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        }
        else // otherwise use a scaled rotation
        {
            targetRotation = maxRotation * rotationSize / slowRadius;
        }

        // the final targetRotation combines speed (already in the variable) and direction
        targetRotation *= rotation / rotationSize;

        // acceleration tries to get to the target rotation
        // something is breaking my angularVelocty... check if NaN and use 0 if so
        float currentAngularVelocity = float.IsNaN(character.angularVelocity) ? 0f : character.angularVelocity;
        result.angular = targetRotation - currentAngularVelocity;
        result.angular /= timeToTarget;

        // check if the acceleration is too great
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }

        result.linear = Vector3.zero;
        return result;
    }
}
