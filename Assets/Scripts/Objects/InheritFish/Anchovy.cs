using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchovy : Fish
{
    [Range(0, 5)] public float _ego;
    [Range(0, 5)] public float _cohesion;
    [Range(0, 5)] public float _alignment;
    [Range(0, 5)] public float _separation;


    BoidsTest test;
    protected override void Initialize()
    {
        rig = GetComponent<Rigidbody>();


        if (playerable == Playerable.Player)
        {
            Initialize_Ability(abilityType.Keep, 240, 4, 0.4f);
        }
        else
        {
            groundLayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Water") | LayerMask.GetMask("Boundary");
            area = FindObjectOfType<Boundary>().GetBoundaryData();

            Initialize_Stat(1, 5, 2, 100, 10);
            Initialize_Weight(_cohesion, _alignment, _separation);

            SetRandomValue();
            StartCoroutine(RandomValueRepeater());
        }


        test = FindObjectOfType<BoidsTest>();
    }

    protected override void AbilityStart()
    {
        moveSpeed = 8f;
        abilityGage--;
    }

    protected override void AbilityOver()
    {
        moveSpeed = 5;
    }



    protected override void FixedUpdate_NonPlayerable()
    {
        _ego = test.egoWeight;
        _cohesion = test.cohesionWeight;
        _alignment = test.alignmentWeight;
        _separation = test.separationWeight;

        CollisionInteract();

        Vector3 ego = EgoVector() * _ego;
        Vector3 cohesion = CalculateCohesionVector() * _cohesion;
        Vector3 alignment = CalculateAlignmentVector() * _alignment;
        Vector3 separation = CalculateSeparationVector() * _separation;

        switch (state)
        {
            case State.Wander:
                moveDir = ego + cohesion + alignment + separation;

                break;
            case State.Sleep:
                break;
            case State.Activity:
                break;
            case State.Food:
                break;
            case State.Chasing:
                break;
            case State.Runaway:
                break;
            case State.Attack:
                break;
            case State.Dead:
                break;
            default:
                break;
        }

        MoveSelf();
    }


    Vector3 ranDir;
    float ranSpd;
    void SetRandomValue()
    {
        ranDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), 0);
        ranSpd = Random.Range(0.1f, moveSpeed);
    }

    protected override Vector3 EgoVector()
    {
        currentSpeed = ranSpd;
        return ranDir;
    }

    IEnumerator RandomValueRepeater()
    {
        while (state == State.Wander)
        {
            yield return new WaitForSeconds(10);
            SetRandomValue();
        }
    }

    //protected override void MoveSelf()
    //{
    //    rig.AddForce(ranDir.normalized * Time.deltaTime * ranSpd * forceNormal);

    //    if (ranDir.x > 0)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(ranDir.x, ranDir.y) * -Mathf.Rad2Deg),
    //            Time.deltaTime * rotaSpeed);
    //    }
    //    else
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(ranDir.x, ranDir.y) * -Mathf.Rad2Deg)),
    //            Time.deltaTime * rotaSpeed);
    //    }
    //}


    int groundLayer;
    RaycastHit hit;
    Boundary.Data area;
    void CollisionInteract()
    {
        Debug.DrawRay(transform.position, ranDir.normalized * 1);
        if (Physics.Raycast(transform.position, ranDir, out hit, 1, LayerMask.GetMask("Ground")))
        {
            Debug.Log(hit.collider.gameObject.name);
            StopAllCoroutines();
            SetRandomValue();
            StartCoroutine(RandomValueRepeater());
            //? 법선벡터, 리플렉션, 히트포인트 등등 디버그
            //Vector3 reflectVec = Vector3.Reflect(hit.point - transform.position, hit.normal);
            //Debug.DrawLine(transform.position, hit.point, Color.red, 5);
            //Debug.DrawRay(hit.point, reflectVec, Color.green, 5);
            //Debug.DrawRay(hit.point, hit.normal, Color.blue, 5);
            //Time.timeScale = 0;
        }
        else if (Physics.Raycast(transform.position, ranDir, out hit, 1, LayerMask.GetMask("Water")))
        {
            StopAllCoroutines();
            ranDir = new Vector3(ranDir.x, -ranDir.y, 0);
            StartCoroutine(RandomValueRepeater());
        }

        if (!Physics.CheckSphere(transform.position, 1, LayerMask.GetMask("Boundary")))
        {
            //Debug.Log("범위 벗어남");
            ranDir = (area.pos + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0)) - transform.position;
        }

    }



}
