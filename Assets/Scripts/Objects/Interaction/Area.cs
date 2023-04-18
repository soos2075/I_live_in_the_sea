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
        
    }

    //? 얘가 AreaManager한테서 미리 Boundary List를 받아서 가지고 있을수도 있음.
    //? 만약 랜덤으로갈지 가까운곳으로 갈지, 그리고 코루틴 시간까지 조정하고싶다면 여기서 작업하고 Fish도 Area에서 받아가면 됨.


}
