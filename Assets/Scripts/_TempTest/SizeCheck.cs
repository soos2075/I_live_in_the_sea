using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeCheck : MonoBehaviour
{

    public bool showGizmos;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            //? 1cm미만 플랑크톤 레이어 
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.1f);
            Gizmos.DrawWireSphere(Vector3.zero, 0.05f);


            //? 1cm~100cm Fish 레이어
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 10f);
            Gizmos.DrawWireSphere(Vector3.zero, 5f);


            //? 30cm~300cm Predator 레이어
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 3f);
            Gizmos.DrawWireSphere(Vector3.zero, 1.5f);
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 30f);
            Gizmos.DrawWireSphere(Vector3.zero, 15f);


            //? 100cm~2000cm ApexPredator 레이어
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 10f);
            Gizmos.DrawWireSphere(Vector3.zero, 5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 100);
            Gizmos.DrawWireSphere(Vector3.zero, 50);


        }
    }
}
