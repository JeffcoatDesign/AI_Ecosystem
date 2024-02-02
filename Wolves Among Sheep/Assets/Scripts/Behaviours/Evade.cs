using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Pursue
{
    float maxPredictionTime = 1f;
    protected override Vector3 getTargetPosition()
    {
        // 1. figure out how far ahead in time we should predict
        Vector3 directionToTarget = character.transform.position - target.transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        float mySpeed = character.linearVelocity.magnitude;
        float predictionTime;
        Kinematic myMovingTarget = target.GetComponent<Kinematic>();
        if (myMovingTarget == null)
        {
            // default to seek behavior for non-kinematic targets
            return base.getTargetPosition();
        }
        if (mySpeed <= distanceToTarget / maxPredictionTime)
        {
            // if I'm far enough away, I can use the max prediction time
            predictionTime = maxPredictionTime;
        }
        else
        {
            predictionTime = distanceToTarget / myMovingTarget.linearVelocity.magnitude;
        }

        return character.transform.position + myMovingTarget.linearVelocity * predictionTime;
    }
}
