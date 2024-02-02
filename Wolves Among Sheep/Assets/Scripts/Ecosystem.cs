using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Ecosystem : MonoBehaviour
{
    public static Ecosystem Instance;

    [SerializeField] private Transform m_boundingPoint1;
    [SerializeField] private Transform m_boundingPoint2;
    [SerializeField] private int m_numGrassToSpawn = 20;
    [SerializeField] private int m_sheepPopulation = 3;
    [SerializeField] private int m_wolfPopulation = 2;
    [SerializeField] private float m_dayLength = 100f;
    [SerializeField] private GameObject m_SheepPrefab;
    [SerializeField] private GameObject m_wolfPrefab;

    public List<GameObject> grassPrefabs;
    public List<Grass> grassList;
    public List<Sheep> sheepList;
    public List<Wolf> wolfList;
    public int sheepMated = 0;
    public int sheepKilled = 0;
    public int wolvesMated = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(DayCycle());
    }

    private IEnumerator DayCycle()
    {
        while (true)
        {
            SpawnGrass();
            SpawnSheep();
            SpawnWolves();
            yield return new WaitForSeconds(m_dayLength);
            DespawnGrass();
            DespawnSheep();
            DespawnWolves();
        }
    }

    private Vector3 GetRandomPointInBounds()
    {
        Vector3 result;
        float minX = m_boundingPoint1.position.x < m_boundingPoint2.position.x ? m_boundingPoint1.position.x : m_boundingPoint2.position.x;
        float maxX = m_boundingPoint1.position.x > m_boundingPoint2.position.x ? m_boundingPoint1.position.x : m_boundingPoint2.position.x;
        float minZ = m_boundingPoint1.position.z < m_boundingPoint2.position.z ? m_boundingPoint1.position.z : m_boundingPoint2.position.z;
        float maxZ = m_boundingPoint1.position.z > m_boundingPoint2.position.z ? m_boundingPoint1.position.z : m_boundingPoint2.position.z;
        result.x = Random.Range(minX, maxX);
        result.y = m_boundingPoint1.position.y;
        result.z = Random.Range(minZ, maxZ);
        return result;
    }

    public bool CheckIfPointInBounds (Vector3 point)
    {
        float minX = m_boundingPoint1.position.x < m_boundingPoint2.position.x ? m_boundingPoint1.position.x : m_boundingPoint2.position.x;
        float maxX = m_boundingPoint1.position.x > m_boundingPoint2.position.x ? m_boundingPoint1.position.x : m_boundingPoint2.position.x;
        float minZ = m_boundingPoint1.position.z < m_boundingPoint2.position.z ? m_boundingPoint1.position.z : m_boundingPoint2.position.z;
        float maxZ = m_boundingPoint1.position.z > m_boundingPoint2.position.z ? m_boundingPoint1.position.z : m_boundingPoint2.position.z;
        return (point.x > minX && point.x < maxX) && (point.z > minZ && point.z < maxX);
    }

    private void SpawnGrass()
    {
        int count = grassPrefabs.Count;
        GameObject[] gameObjects = grassPrefabs.ToArray();
        for (int i = 0; i < m_numGrassToSpawn; ++i)
        {
            int randomIndex = Random.Range(0, count);
            GameObject selectedPrefab = gameObjects[randomIndex];
            Grass newGrass = Instantiate(selectedPrefab).GetComponent<Grass>();
            newGrass.transform.position = GetRandomPointInBounds();
            grassList.Add(newGrass);
        }
    }

    private void DespawnGrass() {
        while (grassList.Count > 0)
        {
            GameObject grassToRemove = grassList[0].gameObject;
            grassList.RemoveAt(0);
            Destroy(grassToRemove);
        }
    }

    private void SpawnSheep ()
    {
        sheepMated = 0;
        for (int i = 0; i < m_sheepPopulation; i++)
        {
            Sheep spawnedSheep = Instantiate(m_SheepPrefab).GetComponent<Sheep>();
            spawnedSheep.transform.position = GetRandomPointInBounds();
            sheepList.Add(spawnedSheep);
        }
    }

    private void DespawnSheep()
    {
        m_sheepPopulation = sheepList.FindAll(x => x.hasEaten).Count();
        m_sheepPopulation += sheepMated;
        m_sheepPopulation -= sheepKilled;
        while (sheepList.Count > 0)
        {
            GameObject sheepToRemove = sheepList[0].gameObject;
            sheepList.RemoveAt(0);
            Destroy(sheepToRemove);
        }
    }

    private void SpawnWolves ()
    {
        wolvesMated = 0;
        for (int i = 0; i < m_wolfPopulation; i++)
        {
            Wolf spawnedWolf = Instantiate(m_wolfPrefab).GetComponent<Wolf>();
            spawnedWolf.transform.position = GetRandomPointInBounds();
            wolfList.Add(spawnedWolf);
        }
    }

    private void DespawnWolves()
    {
        m_wolfPopulation = wolfList.FindAll(x => x.hasEaten).Count();
        m_wolfPopulation += wolvesMated;
        while (wolfList.Count > 0)
        {
            GameObject wolfToRemove = wolfList[0].gameObject;
            wolfList.RemoveAt(0);
            Destroy(wolfToRemove);
        }
    }

    public Grass FindClosestGrass(Vector3 position)
    {
        Grass closet = null;
        float closestDistance = float.MaxValue;
        foreach (var grass in grassList)
        {
            if (grass.hasBeenEaten) continue;
            float currentDistance = Vector3.Distance(position, grass.transform.position);
            if (currentDistance < closestDistance)
            {
                closet = grass;
                closestDistance = currentDistance;
            }
        }
        return closet;
    }

    public Sheep FindPotentialSheepMate(Sheep exclude)
    {
        Sheep closet = null;
        float closestDistance = float.MaxValue;
        foreach (var sheep in sheepList)
        {
            if (sheep == exclude || !sheep.hasEaten || sheep.hasMated || sheep.hasBeenEaten) continue;
            float currentDistance = Vector3.Distance(exclude.transform.position, sheep.transform.position);
            if (currentDistance < closestDistance)
            {
                closet = sheep;
                closestDistance = currentDistance;
            }
        }
        return closet;
    }
    public Wolf FindPotentialWolfMate(Wolf exclude)
    {
        Wolf closet = null;
        float closestDistance = float.MaxValue;
        foreach (var wolf in wolfList)
        {
            if (wolf == exclude || wolf.hasMated) continue;
            float currentDistance = Vector3.Distance(exclude.transform.position, wolf.transform.position);
            if (currentDistance < closestDistance)
            {
                closet = wolf;
                closestDistance = currentDistance;
            }
        }
        return closet;
    }
    public Sheep FindClosestSheep(Transform point)
    {
        Sheep closet = null;
        float closestDistance = float.MaxValue;
        foreach (var sheep in sheepList)
        {
            if (!sheep.hasEaten || sheep.hasMated || sheep.hasBeenEaten) continue;
            float currentDistance = Vector3.Distance(point.position, sheep.transform.position);
            if (currentDistance < closestDistance)
            {
                closet = sheep;
                closestDistance = currentDistance;
            }
        }
        return closet;
    }
    public Wolf FindClosestWolf(Transform point)
    {
        Wolf closet = null;
        float closestDistance = float.MaxValue;
        foreach (var wolf in wolfList)
        {
            float currentDistance = Vector3.Distance(point.position, wolf.transform.position);
            if (currentDistance < closestDistance)
            {
                closet = wolf;
                closestDistance = currentDistance;
            }
        }
        return closet;
    }
}
