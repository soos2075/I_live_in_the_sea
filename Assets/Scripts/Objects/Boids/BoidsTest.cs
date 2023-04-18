using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsTest : MonoBehaviour
{

    public Transform pos;

    Vector3 ranVec;
    Quaternion ranRot;

    public List<GameObject> objList;

    public List<int> quantityList;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        for (int i = 0; i < objList.Count; i++)
        {
            for (int j = 0; j < quantityList[i]; j++)
            {

                ranVec = Random.insideUnitSphere * 5 * Vector2.one;
                ranRot = Quaternion.Euler(0, 0, Random.Range(0, 360));
                var aaa = Instantiate(objList[i], ranVec + pos.position, ranRot);
            }
        }
    }



    void Update()
    {
        
    }
}
