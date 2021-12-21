using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSparePartSpawn : MonoBehaviour
{
    public static sSparePartSpawn instance;

    public Transform[] spawnPoints;
    public GameObject[] spareParts;
  //  public int whatSparePart;

    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnSpareParts(int arrayIndex)
    {
        GameObject.Instantiate(spareParts[arrayIndex], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Random.rotation);
    }
}
 