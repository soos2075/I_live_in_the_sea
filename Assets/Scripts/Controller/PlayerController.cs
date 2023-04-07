using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Fish fish;
    Rigidbody rig;

    [SerializeField] float size;
    [SerializeField] float addForce;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        InitializeFishData();
    }

    void InitializeFishData()
    {
        fish = GetComponent<Fish>();
        if (fish)
        {
            fish.playerable = Fish.Playerable.Player;
            size = fish.size;
        }
    }

    void Update()
    {
        //CheckOcean();
        if (fish && Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space))
        {
            fish.Ability();
        }
    }

    private void FixedUpdate()
    {
        if (rig.useGravity)
        {
            addForce = fish.forceWeak;
        }
        else
        {
            addForce = fish.forceNormal;
        }
        PlayerMoveKeyboard();
    }

    void PlayerMoveKeyboard()
    {
        //? 기본이동
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotaSpeed);
        //transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);

        //? 이동
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            rig.AddForce(moveDir.normalized * Time.deltaTime * fish.moveSpeed * addForce);
        }

        //? 각도
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * fish.rotaSpeed);
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * fish.rotaSpeed);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(transform.eulerAngles.y) > 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * fish.rotaSpeed);
            }
            else if (Mathf.Abs(transform.eulerAngles.y) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * fish.rotaSpeed);
            }
        }
    }

    //void CheckOcean()
    //{
    //    if (Physics.Raycast(transform.position, Vector3.down, 100, LayerMask.GetMask("Water")))
    //    {
    //        rig.useGravity = true;
    //    }
    //    else
    //    {
    //        rig.useGravity = false;
    //    }
    //}

}
