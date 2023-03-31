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
        
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (chasingTarget == null)
        {
            return;
        }
        //transform.position = chasingTarget.position;


        transform.position = Vector3.Lerp(transform.position, chasingTarget.position, Time.deltaTime * camSpeed);


    }
}
