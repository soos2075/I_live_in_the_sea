using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public Transform chasingTarget;
    Camera cam;

    public float camSpeed = 3;


    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }
    void Start()
    {
        if (chasingTarget == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go)
            {
                chasingTarget = go.transform;
            }
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (chasingTarget == null)
        {
            return;
        }
        //transform.position = chasingTarget.position;


        transform.position = Vector3.Lerp(transform.position, chasingTarget.position, Time.deltaTime * camSpeed);

        CheckOcean();
    }


    void CheckOcean()
    {
        Vector3 dir = Vector3.Normalize(transform.position - cam.transform.position);

        //Debug.DrawRay(cam.transform.position, dir * -cam.transform.position.z, Color.red);

        if (Physics.Raycast(cam.transform.position, dir, -cam.transform.position.z, LayerMask.GetMask("Water")))
        {
            RenderSettings.fog = false;
        }
        else if(Physics.Raycast(cam.transform.position, Vector3.down, 100, LayerMask.GetMask("Water")))
        {
            RenderSettings.fog = false;
        }
        else
        {
            RenderSettings.fog = true;
        }

    }

}
