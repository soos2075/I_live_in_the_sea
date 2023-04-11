using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsTest : MonoBehaviour
{
    public GameObject obj;
    public int Quantity;
    public Transform pos;

    public GameObject obj2;
    public int Quantity2;
    public Transform pos2;

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

            var aaa = Instantiate(obj, ranVec + pos.position, ranRot);
            //if (i % 10 == 0)
            //{
            //    aaa.GetComponent<Anchovy>()._ego = 5;
            //}
        }

        for (int i = 0; i < Quantity2; i++)
        {
            ranVec = Random.insideUnitSphere * 5 * Vector2.one;
            ranRot = Quaternion.Euler(0, 0, Random.Range(0, 360));

            var aaa = Instantiate(obj2, ranVec + pos2.position, ranRot);
        }
    }

    void Update()
    {
        
    }
}
