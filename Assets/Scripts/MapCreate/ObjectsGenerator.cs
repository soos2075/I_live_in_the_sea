using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//? 런타임 아닐때도 컴파일되면 알아서 start함수 실행됨
//[ExecuteInEditMode] 
public class ObjectsGenerator : MonoBehaviour
{
    public List<GameObject> objList;

    public Transform pos_X1;
    public Transform pos_X2;


    public int quantity;


    public Vector2 ranPosition_x;
    public Vector2 ranPosition_z;
    public float ranRotation_y;
    public Vector2 ranScale;

    public string CreateCanvasLayer;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CreatePosRaycast()
    {
        float offset_X = (pos_X2.position.x - pos_X1.position.x) / quantity;

        if (isRandom)
        {
            for (int i = 0; i < quantity; i++)
            {
                Ray ray = new Ray(new Vector3(pos_X1.position.x, 0, 0) + 
                    new Vector3(offset_X * i, 0, 0) + 
                    new Vector3(Random.Range(ranPosition_x.x, ranPosition_x.y), 0, Random.Range(ranPosition_z.x, ranPosition_z.y))
                    , Vector3.down);

                RaycastHit[] hits = Physics.RaycastAll(ray, 1000, LayerMask.GetMask(CreateCanvasLayer));

                if (hits.Length > 0)
                {
                    float _height = -1000;
                    int _num = 0;
                    for (int k = 0; k < hits.Length; k++)
                    {
                        if (hits[k].point.y > _height)
                        {
                            _height = hits[k].point.y;
                            _num = k;
                        }
                    }
                    CreateObject(hits[_num].point);
                }
            }
        }
        //else
        //{
        //    for (int i = 0; i < quantity; i++)
        //    {
        //        Ray ray = new Ray(new Vector3(pos_X1.position.x, 0, 0) + new Vector3(offset_X * i, 0, 0), Vector3.down);
        //        //Debug.DrawRay(pos_X1.position + new Vector3(offset_X * i, 0, 0), Vector3.down * 1000, Color.red, 10);
        //        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, LayerMask.GetMask(CreateCanvasLayer));
        //        foreach (var hit in hits)
        //        {
        //            CreateObject(hit.point);
        //        }
        //    }
        //}
    }

    public bool isRandom;

    public string rootGroupName;
    Transform CreateRoot
    {
        get 
        {
            GameObject root = GameObject.Find(rootGroupName);
            if (root != null)
            {
                return root.transform;
            }
            else
            {
                root = new GameObject { name = rootGroupName };
                return root.transform;
            }
        }
    }
    void CreateObject(Vector3 createPos)
    {
        if (isRandom)
        {
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(objList[Random.Range(0, objList.Count)], CreateRoot);
            obj.transform.position = createPos;
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, ranRotation_y), 0);
            obj.transform.localScale *= Random.Range(ranScale.x, ranScale.y);
        }
        else
        {
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(objList[Random.Range(0, objList.Count)], CreateRoot);
            obj.transform.position = createPos;
        }

    }


}
