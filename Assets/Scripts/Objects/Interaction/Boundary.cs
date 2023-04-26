using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public enum BoundaryType
    {
        Sphere,
        Box,
        Capsule,
    }

    public Collider col;
    Data data;

    public class Data
    {
        public BoundaryType type;
        public Vector3 centerPos;

        public float radius;

        public Vector3 boxSize;

        public float height;
        public int capsuleOption;

        public Data(BoundaryType t, Vector3 pos, float r, Vector3 box_size, float _height = 0, int cap_Axis = 0)
        {
            type = t;
            centerPos = pos;
            radius = r;
            boxSize = box_size;

            height = _height;
            capsuleOption = cap_Axis;
        }


        public Vector3 GetRandomPosition()
        {
            Vector3 vec = centerPos;
            switch (type)
            {
                case BoundaryType.Sphere:
                    vec = new Vector3(
                        Random.Range(centerPos.x - radius, centerPos.x + radius),
                        Random.Range(centerPos.y - radius, centerPos.y + radius),
                        0);
                    break;

                case BoundaryType.Box:
                    vec = new Vector3(
                        Random.Range(centerPos.x - (boxSize.x * 0.5f), centerPos.x + (boxSize.x * 0.5f)),
                        Random.Range(centerPos.y - (boxSize.y * 0.5f), centerPos.y + (boxSize.y * 0.5f)),
                        0);
                    break;

                case BoundaryType.Capsule:
                    vec = new Vector3(
                        Random.Range(centerPos.x - (height * 0.5f), centerPos.x + (height * 0.5f)),
                        Random.Range(centerPos.y - radius, centerPos.y + radius),
                        0);
                    break;
            }

            return vec;
        }
    }

    private void Awake()
    {
        col = GetComponent<Collider>();
        SphereCollider sph = col as SphereCollider;
        BoxCollider box = col as BoxCollider;
        CapsuleCollider cap = col as CapsuleCollider;

        if (sph)
        {
            data = new Data(BoundaryType.Sphere, transform.position + sph.center, sph.radius, Vector3.zero);
        }
        else if(box)
        {
            data = new Data(BoundaryType.Box, transform.position + box.center, box.size.x + box.size.y, box.size);
        }
        else if (cap)
        {
            data = new Data(BoundaryType.Capsule, transform.position + cap.center, cap.radius, Vector3.zero, cap.height, cap.direction);
        }
    }
    private void Start()
    {

    }


    public Data GetBoundaryData()
    {
        return data;
    }



    public bool showGizmo;

    private void OnDrawGizmos()
    {
        //? collider.bounds.center는 월드좌표, collider.center는 로컬좌표로 나오나봄?
        if (showGizmo)
        {
            //Debug.Log(GetComponent<Collider>().bounds.center);

            Vector3 cen = GetComponent<Collider>().bounds.center;
            float sizeX = GetComponent<Collider>().bounds.size.x;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(cen, new Vector3(sizeX, 1, 1));
        }
    }


}
