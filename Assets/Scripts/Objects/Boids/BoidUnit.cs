using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidUnit : MonoBehaviour
{
    public float moveSpd;
    public Vector3 targetVec;

    public List<BoidUnit> neighbours = new List<BoidUnit>();
    public float neighbourRadius;

    BoidsTest boids;
    Rigidbody rig;

    void Start()
    {
        boids = FindObjectOfType<BoidsTest>();
        rig = GetComponent<Rigidbody>();

        StartCoroutine("FindNeighbourCoroutine");
        targetVec = transform.forward;
    }

    void Update()
    {
        Vector3 cohesionVec = CalculateCohesionVector() * boids.cohesionWeight;
        Vector3 alignmentVec = CalculateAlignmentVector() * boids.alignmentWeight;
        Vector3 separationVec = CalculateSeparationVector() * boids.separationWeight;

        targetVec = cohesionVec + alignmentVec + separationVec;


        targetVec = Vector3.Lerp(this.transform.forward, targetVec, Time.deltaTime);
        targetVec = targetVec.normalized;

        if (!Physics.CheckSphere(transform.position, 1, LayerMask.GetMask("Boundary")))
        {
            //Debug.Log("범위 벗어남");
            targetVec = Vector3.zero - transform.position;
        }



        transform.rotation = Quaternion.LookRotation(targetVec);
        rig.AddForce(targetVec * moveSpd * Time.deltaTime * 100);
    }


    Coroutine findNeighbourCoroutine;
    //? 응집 - Cohesion,  
    IEnumerator FindNeighbourCoroutine()
    {
        neighbours.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, neighbourRadius, LayerMask.GetMask("BoidsTest"));
        for (int i = 0; i < colls.Length; i++)
        {
            neighbours.Add(colls[i].GetComponent<BoidUnit>());
        }
        yield return new WaitForSeconds(Random.Range(1.0f, 1.0f));
        findNeighbourCoroutine = StartCoroutine("FindNeighbourCoroutine");
    }
    private Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 위치 더하기
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            // 이웃이 없으면 vector3.zero 반환
            return cohesionVec;
        }

        // 중심 위치로의 벡터 찾기
        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        cohesionVec.Normalize();
        return cohesionVec;
    }

    //? 정렬 - Alignment,
    private Vector3 CalculateAlignmentVector()
    {
        Vector3 alignmentVec = transform.forward;
        if (neighbours.Count > 0)
        {
            // 이웃들이 향하는 방향의 평균 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].transform.forward;
            }
        }
        else
        {
            // 이웃이 없으면 그냥 forward로 이동
            return alignmentVec;
        }

        alignmentVec /= neighbours.Count;
        alignmentVec.Normalize();
        return alignmentVec;
    }

    //? 분리 - Separation
    private Vector3 CalculateSeparationVector()
    {
        Vector3 separationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃들을 피하는 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                separationVec += (transform.position - neighbours[i].transform.position);
            }
        }
        else
        {
            // 이웃이 없으면 vector.zero 반환
            return separationVec;
        }
        separationVec /= neighbours.Count;
        separationVec.Normalize();
        return separationVec;
    }

}
