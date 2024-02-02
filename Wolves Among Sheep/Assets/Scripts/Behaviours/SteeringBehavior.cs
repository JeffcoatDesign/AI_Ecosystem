using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehavior
{
    public abstract SteeringOutput getSteering();
    public GameObject target;
    public Kinematic character;
}
