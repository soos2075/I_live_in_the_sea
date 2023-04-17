using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{


    public List<string> areaList;
    public int areaLayer { get; set; }


    private void Awake()
    {
        foreach (var str in areaList)
        {
            areaLayer |= LayerMask.GetMask(str);
        }
    }
    void Start()
    {
        SetBoundary();
    }


    void SetBoundary()
    {
        var lay = GameManager.Area.AllBoundaryList;


        foreach (var bound in GameManager.Area.AllBoundaryList)
        {
            if (bound.CompareTag("CoralArea_A"))
            {

                //boundaryData = bound.GetBoundaryData();
                break;
            }
        }

        for (int i = 0; i < areaList.Count; i++)
        {

        }



    }


}
