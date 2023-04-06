using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchovy : Fish
{

    protected override void Initialize()
    {
        groundLayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Water") | LayerMask.GetMask("Boundary");
        area = FindObjectOfType<Boundary>().GetBoundaryData();

        Initialize_Stat(1, 5, 2, 100, 10);
        Initialize_Ability(abilityType.Keep, 240, 4, 0.4f);
        rig = GetComponent<Rigidbody>();

        SetRandomValue();
        StartCoroutine(RandomValueRepeater());
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
        base.FixedUpdate_NonPlayerable();
        CollisionInteract();
    }


    Rigidbody rig;
    Vector3 ranDir;
    float ranSpd;
    void SetRandomValue()
    {
        ranDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.75f, 0.75f), 0);
        ranSpd = Random.Range(0.1f, moveSpeed);
    }

    IEnumerator RandomValueRepeater()
    {
        while (state == State.Idle)
        {
            yield return new WaitForSeconds(5);
            SetRandomValue();
        }
    }

    protected override void MoveSelf()
    {
        rig.AddForce(ranDir.normalized * Time.deltaTime * ranSpd * forceNormal);

        if (ranDir.x > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(ranDir.x, ranDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * rotaSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(ranDir.x, ranDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * rotaSpeed);
        }
    }


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
