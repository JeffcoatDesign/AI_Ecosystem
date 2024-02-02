using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private int m_foodValue = 1;
    public bool hasBeenEaten = false;
    public void EatGrass (Sheep sheep)
    {
        if (!hasBeenEaten)
        {
            hasBeenEaten = true;
            sheep.AddFood(m_foodValue);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
