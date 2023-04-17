using System.Collections.Generic;
using UnityEngine;

public class AreaManager
{

    public Boundary[] AllBoundaryList;


    public List<Boundary> SurfaceList;
    public List<Boundary> LandList;
    public List<Boundary> SeaweedList;
    public List<Boundary> CoralList;
    public List<Boundary> SandList;
    public List<Boundary> NothingList;



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

    public Boundary GetBoundaryData(int layer)
    {




        return null;
    }






}
