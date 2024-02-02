using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Kinematic
{
    SteeringBehavior myMoveType;
    SteeringBehavior myRotateType;
    public bool hasEaten = false;
    private int foodEaten;
    [SerializeField] private int foodRequired;
    [SerializeField] private float radiusToArriveTarget = 1f;
    [SerializeField] private float radiusToFlee = 5f;
    [SerializeField] private int foodValue = 1;
    GameObject target;
    Grass targetGrass;
    Sheep targetSheep;
    Wolf targetWolf;
    public bool hasBeenEaten = false;
    public AnimalState myState;
    internal bool hasMated = false;

    void Start()
    {
        SetState(AnimalState.FindFood);
    }

    public void EatSheep (Wolf wolf)
    {
        if (!hasBeenEaten)
        {
            hasBeenEaten=true;
            Ecosystem.Instance.sheepKilled++;
            wolf.AddFood(foodValue);
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    protected override void Update()
    {
        steeringUpdate = new SteeringOutput();

        if (target == null)
        {
            FindCorrectTarget();
        }

        if (!hasEaten && myState != AnimalState.FindFood) SetState(AnimalState.FindFood);
        else if (hasEaten && !hasMated && myState != AnimalState.FindMate) SetState(AnimalState.FindMate);
        else if (hasEaten && hasMated && myState != AnimalState.Nothing) SetState(AnimalState.Nothing);

        if (myState == AnimalState.FindFood && targetGrass != null)
        {
            if (Vector3.Distance(transform.position, targetGrass.transform.position) < radiusToArriveTarget && !targetGrass.hasBeenEaten)
                targetGrass.EatGrass(this);
            else FindCorrectTarget();
        }
        else if (myState == AnimalState.FindMate && targetSheep != null)
        {
            if (Vector3.Distance(transform.position, targetSheep.transform.position) < radiusToArriveTarget && !hasMated && !targetSheep.hasMated && !targetSheep.hasBeenEaten)
            {
                targetSheep.hasMated = true;
                hasMated = true;
                Ecosystem.Instance.sheepMated++;
            }
            else
                FindCorrectTarget();
        }
        targetWolf = Ecosystem.Instance.FindClosestWolf(transform);
        if (targetWolf != null && Vector3.Distance(transform.position, targetWolf.transform.position) < radiusToFlee)
            SetState(AnimalState.Flee);
        if (!Ecosystem.Instance.CheckIfPointInBounds(transform.position))
        {
            SetState(AnimalState.Return);
        }

        if (myMoveType != null)
            steeringUpdate.linear = myMoveType.getSteering().linear;
        if (myRotateType != null)
            steeringUpdate.angular = myRotateType.getSteering().angular;
            base.Update();
    }

    private void SetTarget(GameObject target)
    {
        if (myMoveType != null)
            myMoveType.target = target;
        if (myRotateType != null)
            myRotateType.target = target;
    }

    private void SetArrive()
    {
        Arrive arrive = new Arrive();
        arrive.targetRadius = radiusToArriveTarget;
        myMoveType = arrive;
        myMoveType.character = this;
    }

    private void SetPursue()
    {
        Pursue pursue = new Pursue();
        pursue.character = this;
        pursue.target = target;
        myMoveType = pursue;
    }
    private void SetSeek()
    {
        Seek seek = new ();
        seek.character = this;
        seek.target = target;
        myMoveType = seek;
    }
    private void SetEvade()
    {
        Evade evade = new Evade();
        evade.character = this;
        evade.target = target;
        myMoveType = evade;
    }

    private void SetWander()
    {
        Wander wander = new Wander();
        wander.character = this;
        wander.target = target;
        myMoveType = wander;
    }

    private void SetLookWhereGoing()
    {
        LookWhereGoing lookWhereGoing = new LookWhereGoing();
        lookWhereGoing.target = myMoveType.target;
        lookWhereGoing.character = this;
        myRotateType = lookWhereGoing;
    }

    private void SetFace()
    {
        Face face = new Face();
        face.target = target;
        face.character = this;
        myRotateType = face;
    }

    private void SetState(AnimalState state)
    {
        myState = state;
        if (myState == AnimalState.FindFood)
        {
            SetArrive();
            SetFace();
        }
        else if (myState == AnimalState.FindMate)
        {
            SetSeek();
            SetFace();
        } 
        else if (myState == AnimalState.Flee)
        {
            SetEvade();
            SetLookWhereGoing();
        } 
        else if (myState == AnimalState.Return)
        {
            SetArrive();
            SetFace();
        }
        else if (myState == AnimalState.Nothing)
        {
            SetWander();
            SetLookWhereGoing();
        }
        FindCorrectTarget();
    }

    private void FindCorrectTarget()
    {
        target = null;
        if (myState == AnimalState.FindFood)
        {
            targetGrass = Ecosystem.Instance.FindClosestGrass(transform.position);
            if (targetGrass != null)
                target = targetGrass.gameObject;
        }
        else if (myState == AnimalState.FindMate)
        {
            targetSheep = Ecosystem.Instance.FindPotentialSheepMate(this);
            if (targetSheep != null)
                target = targetSheep.gameObject;
        }
        else if (myState == AnimalState.Flee)
        {
            targetWolf = Ecosystem.Instance.FindClosestWolf(transform);
            if (targetWolf != null)
                target = targetWolf.gameObject;
        }
        else if (myState == AnimalState.Return)
        {
            target = Ecosystem.Instance.gameObject;
        }
        else
            target = Ecosystem.Instance.gameObject;
        SetTarget(target);
    }

    internal void AddFood(int m_foodValue)
    {
        foodEaten += m_foodValue;
        hasEaten = foodEaten >= foodRequired;
        if (hasEaten)
        {
            SetState(AnimalState.FindMate);
        }
    }
}

public enum AnimalState {
    FindFood,
    FindMate,
    Flee,
    Return,
    Nothing
}
