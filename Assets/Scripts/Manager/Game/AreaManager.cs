using System.Collections.Generic;
using UnityEngine;

public class AreaManager
{

    private Boundary[] AllBoundaryList;


    private List<Boundary> SurfaceList;
    private List<Boundary> LandList;
    private List<Boundary> SeaweedList;
    private List<Boundary> CoralList;
    private List<Boundary> SandList;
    private List<Boundary> NothingList;


    public void Init()
    {
        AllBoundaryList = GameManager.FindObjectsOfType<Boundary>();

        SurfaceList = new List<Boundary>();
        LandList = new List<Boundary>();
        SeaweedList = new List<Boundary>();
        CoralList = new List<Boundary>();
        SandList = new List<Boundary>();
        NothingList = new List<Boundary>();

        foreach (var area in AllBoundaryList)
        {
            if (area.gameObject.layer == LayerMask.NameToLayer("Area_Surface"))
            {
                SurfaceList.Add(area);
            }
            else if (area.gameObject.layer == LayerMask.NameToLayer("Area_Land"))
            {
                LandList.Add(area);
            }
            else if (area.gameObject.layer == LayerMask.NameToLayer("Area_Seaweed"))
            {
                SeaweedList.Add(area);
            }
            else if (area.gameObject.layer == LayerMask.NameToLayer("Area_Coral"))
            {
                CoralList.Add(area);
            }
            else if (area.gameObject.layer == LayerMask.NameToLayer("Area_Sand"))
            {
                SandList.Add(area);
            }
            else if (area.gameObject.layer == LayerMask.NameToLayer("Area_Nothing"))
            {
                NothingList.Add(area);
            }
        }

    }

    public List<Boundary> GetBoundaryList(int layer)
    {
        List<Boundary> list = new List<Boundary>();

        if ((layer & LayerMask.GetMask("Area_Surface")) > 0)
        {
            for (int i = 0; i < SurfaceList.Count; i++)
            {
                list.Add(SurfaceList[i]);
            }
        }

        if ((layer & LayerMask.GetMask("Area_Land")) > 0)
        {
            for (int i = 0; i < LandList.Count; i++)
            {
                list.Add(LandList[i]);
            }
        }

        if ((layer & LayerMask.GetMask("Area_Seaweed")) > 0)
        {
            for (int i = 0; i < SeaweedList.Count; i++)
            {
                list.Add(SeaweedList[i]);
            }
        }

        if ((layer & LayerMask.GetMask("Area_Coral")) > 0)
        {
            for (int i = 0; i < CoralList.Count; i++)
            {
                list.Add(CoralList[i]);
            }
        }

        if ((layer & LayerMask.GetMask("Area_Sand")) > 0)
        {
            for (int i = 0; i < SandList.Count; i++)
            {
                list.Add(SandList[i]);
            }
        }

        if ((layer & LayerMask.GetMask("Area_Nothing")) > 0)
        {
            for (int i = 0; i < NothingList.Count; i++)
            {
                list.Add(NothingList[i]);
            }
        }

        return list;
    }


    public Boundary GetRandomBoundary(int layer)
    {
        var list = GetBoundaryList(layer);
        Boundary ranPick = list[Random.Range(0, list.Count)];
        return ranPick;
    }

    public Boundary GetCloseBoundary(int layer, Vector3 position)
    {
        var list = GetBoundaryList(layer);
        if (list.Count == 0)
        {
            Debug.Log("오류@@@");
            return null;
        }

        Boundary closePick = list[0];
        float dist = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            float tempDist = (list[i].transform.position - position).magnitude;
            if (tempDist < dist)
            {
                dist = tempDist;
                closePick = list[i];
            }
        }
        return closePick;
    }


}
