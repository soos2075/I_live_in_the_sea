using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsTest : MonoBehaviour
{
    public GameObject obj;
    public int Quantity;
    public Transform pos;

    Vector3 ranVec;
    Quaternion ranRot;

    [Range(0, 5)]
    public float egoWeight;
    [Range(0, 5)]
    public float cohesionWeight;
    [Range(0, 5)]
    public float alignmentWeight;
    [Range(0, 5)]
    public float separationWeight;

    void Start()
    {
        for (int i = 0; i < Quantity; i++)
        {
            ranVec = Random.insideUnitSphere * 5 * Vector2.one;
            ranRot = Quaternion.Euler(0, 0, Random.Range(0, 360));

            Instantiate(obj, ranVec + pos.position, ranRot);
        }
    }

    void Update()
    {
        
    }
}
