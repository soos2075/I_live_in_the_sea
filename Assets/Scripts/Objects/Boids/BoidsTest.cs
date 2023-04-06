using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsTest : MonoBehaviour
{
    public GameObject obj;

    Vector3 ranVec;
    Quaternion ranRot;

    [Range(0, 5)]
    public float cohesionWeight;
    [Range(0, 5)]
    public float alignmentWeight;
    [Range(0, 5)]
    public float separationWeight;

    void Start()
    {
        for (int i = 0; i < 300; i++)
        {
            ranVec = Random.insideUnitSphere * 20;
            ranRot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), 0);

            Instantiate(obj, ranVec, ranRot);
        }
    }

    void Update()
    {
        
    }
}
