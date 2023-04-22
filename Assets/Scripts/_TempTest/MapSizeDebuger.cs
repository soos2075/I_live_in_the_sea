using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSizeDebuger : MonoBehaviour
{

    public bool show;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    void OnDrawGizmos()
    {
        if (show)
        {
            int maximum = 2000;


            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.up * maximum);
            Gizmos.DrawLine(Vector3.zero, Vector3.down * maximum);


            Gizmos.color = Color.green;
            for (int i = 0; i < 11; i++)
            {
                var vec = Vector3.down * i * 100;

                Gizmos.color = Color.green;

                if (i == 10)
                {
                    Gizmos.color = Color.blue;
                }
                
                Gizmos.DrawLine(vec, new Vector2(maximum, vec.y));
                Gizmos.DrawLine(vec, new Vector2(-maximum, vec.y));

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector2(0, vec.y), new Vector3(maximum, 1, 1));
            }        }


    }

}
