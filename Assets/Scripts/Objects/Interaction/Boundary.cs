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

        public Data(BoundaryType t, Vector3 pos, float r, Vector3 box_size, float _height = 0, int cap_Axis = 1)
        {
            type = t;
            centerPos = pos;
            radius = r;
            boxSize = box_size;

            height = _height;
            capsuleOption = cap_Axis;
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


}
