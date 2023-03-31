using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float rotaSpeed;


    public bool fogSwitch;
    void Start()
    {
        RenderSettings.fogColor = Color.blue;
        //RenderSettings.fog = true;
    }

    void Update()
    {
        RenderSettings.fog = fogSwitch;

        PlayerMoveKeyboard();
    }

    void PlayerMoveKeyboard()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            Vector3 moveDir =
                Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");


            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotaSpeed); //? 방향전환속도

            transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);
        }
    }
}
