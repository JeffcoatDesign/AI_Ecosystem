using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Kinematic
{
    SteeringBehavior myMoveType;
    SteeringBehavior myRotateType;
    public bool hasEaten = false;
    private int foodEaten;
    [SerializeField] private int foodRequired;
    [SerializeField] private float radiusToArriveTarget = 1f;
    GameObject target;
    Sheep targetSheep;
    Wolf targetWolf;
    public AnimalState myState;
    internal bool hasMated = false;

    void Start()
    {
        SetState(AnimalState.FindFood);
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

        if (myState == AnimalState.FindFood && targetSheep != null)
        {
            if (Vector3.Distance(transform.position, targetSheep.transform.position) < radiusToArriveTarget && !targetSheep.hasBeenEaten)
                targetSheep.EatSheep(this);
            else if (targetSheep.hasBeenEaten) FindCorrectTarget();
        }
        else if (myState == AnimalState.FindMate && targetWolf != null)
        {
            if (Vector3.Distance(transform.position, targetWolf.transform.position) < radiusToArriveTarget && !hasMated && !targetWolf.hasMated)
            {
                targetWolf.hasMated = true;
                hasMated = true;
                Ecosystem.Instance.wolvesMated++;
            }
            else
                FindCorrectTarget();
        }
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
            SetPursue();
            SetFace();
        }
        else if (myState == AnimalState.FindMate)
        {
            SetPursue();
            SetFace();
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
            targetSheep = Ecosystem.Instance.FindClosestSheep(transform);
            if (targetSheep != null)
                target = targetSheep.gameObject;
        }
        else if (myState == AnimalState.FindMate)
        {
            targetWolf = Ecosystem.Instance.FindPotentialWolfMate(this);
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