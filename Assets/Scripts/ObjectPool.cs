using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    
    public List<GameObject> pooledWalls; 
    public List<GameObject> pooledColliders;
    
    public GameObject wallPrefab;
    public GameObject colliderPrefab;
    public GameObject powerupPrefab;

    private int wallAmount = 25;
    private int colliderAmount = 50;

    private void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledWalls = new List<GameObject>();
        pooledColliders = new List<GameObject>();

        GameObject tmp;

        for (int i = 0; i < wallAmount; i++)
        {
            tmp = Instantiate(wallPrefab);
            tmp.SetActive(false);
            pooledWalls.Add(tmp);
        }

        for (int i = 0; i < colliderAmount; i++)
        {
            tmp = Instantiate(colliderPrefab);
            tmp.SetActive(false);
            pooledColliders.Add(tmp);
        }
    }

    public GameObject GetPooledWall()
    {
        for (int i = 0; i < wallAmount; i++)
        {
            if (!pooledWalls[i].activeInHierarchy)
            {
                return pooledWalls[i];
            }
        }
        return null;
    }

    public GameObject GetPooledCollider()
    {
        for (int i = 0; i < colliderAmount; i++)
        {
            if (!pooledColliders[i].activeInHierarchy)
            {
                return pooledColliders[i];
            }
        }
        return null;
    }

    // A single powerup should be enough
    public GameObject GetPooledPowerup()
    {
        GameObject tmp = Instantiate(powerupPrefab);
        tmp.SetActive(false);
        return tmp;
    }
}
