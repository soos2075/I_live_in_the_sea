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

    [SerializeField] BoundaryType _type;
    [SerializeField] Vector3 _pos;
    [SerializeField] float _radius;
    [SerializeField] float _xSize;
    [SerializeField] float _ySize;

    [SerializeField] bool _drawGizmos;

    public Collider col;

    public class Data
    {
        public BoundaryType type;
        public Vector3 pos;
        public float radius;
        public float xSize;
        public float ySize;

        public Data(BoundaryType t, Vector3 p, float r, float x, float y)
        {
            type = t;
            pos = p;
            radius = r;
            xSize = x;
            ySize = y;
        }
    }

    Data data;

    public GameObject anc;
    public int quantity;

    private void Awake()
    {
        col = GetComponent<Collider>();
        data = new Data(_type, _pos, _radius, _xSize, _ySize);
    }
    private void Start()
    {
        switch (data.type)
        {
            case BoundaryType.Sphere:
                SphereCollider spc = col as SphereCollider;
                spc.radius = data.radius;

                break;
            case BoundaryType.Box:
                break;
            case BoundaryType.Capsule:
                break;
        }

        for (int i = 0; i < quantity; i++)
        {
            Instantiate(anc, Random.insideUnitSphere * Vector2.one * 5, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
    }
    private void Update()
    {
        _pos = transform.position;
    }


    public Data GetBoundaryData()
    {
        return data;
    }


    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            switch (_type)
            {
                case BoundaryType.Sphere:
                    Gizmos.DrawSphere(_pos, _radius);
                    break;

                case BoundaryType.Box:
                    Gizmos.DrawCube(_pos, new Vector3(_xSize, _ySize, 1));
                    break;

                case BoundaryType.Capsule:
                    break;
            }
        }
    }
}
