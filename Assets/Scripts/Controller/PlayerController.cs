using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float rotaSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        PlayerMoveKeyboard();
    }

    void PlayerMoveKeyboard()
    {
        //? 이동
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotaSpeed);
            transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);
        }

        //? 각도
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            //Vector3 dir = Target.transform.position - transform.position;
            //transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.x, dir.y) * -Mathf.Rad2Deg);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * rotaSpeed);
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
            //Vector3 dir = Target.transform.position - transform.position;
            //transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.x, dir.y) * -Mathf.Rad2Deg);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * rotaSpeed);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(transform.eulerAngles.y) > 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * rotaSpeed);
            }
            else if (Mathf.Abs(transform.eulerAngles.y) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * rotaSpeed);
            }
        }
    }
}
