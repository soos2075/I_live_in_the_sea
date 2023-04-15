using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Fish fish;
    Rigidbody rig;

    [SerializeField] float size;
    [SerializeField] float addForce;

    Transform pos_Tail;
    Transform pos_Head;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        InitializeFishData();

        pos_Tail = transform.GetChild(0);
        pos_Head = transform.GetChild(1);
    }

    void InitializeFishData()
    {
        fish = GetComponent<Fish>();
        if (fish)
        {
            fish.playerable = Fish.Playerable.Player;
            size = fish.Size;
        }
    }

    void Update()
    {
        if (fish && Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space))
        {
            fish.Ability();
        }
    }

    private void FixedUpdate()
    {
        if (rig.useGravity)
        {
            addForce = fish.ForceWeak;
        }
        else
        {
            addForce = fish.ForceNormal;
        }
        
        CollisionCheck();
        PlayerMoveKeyboard();
    }


    void PlayerMoveKeyboard()
    {
        //? 기본이동
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotaSpeed);
        //transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);

        Vector3 moveDir = Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
        //if (moveDir == Vector3.zero)
        //{
        //    moveDir = fish.Coordinate.Front;
        //}

        //? 이동
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            rig.AddForce(moveDir.normalized * Time.deltaTime * fish.MoveSpeed * addForce);
        }

        //? 각도
        float angle_Z = 90 + Mathf.Atan2(moveDir.x, moveDir.y) * -Mathf.Rad2Deg;
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle_Z),
                Time.deltaTime * fish.RotaSpeed);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -angle_Z),
                Time.deltaTime * fish.RotaSpeed);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            if (Mathf.Abs(transform.eulerAngles.y) > 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -angle_Z),
                Time.deltaTime * fish.RotaSpeed);
            }
            else if (Mathf.Abs(transform.eulerAngles.y) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle_Z),
                Time.deltaTime * fish.RotaSpeed);
            }
        }

        transform.rotation = Quaternion.LookRotation(new Vector3(0, moveDir.y, moveDir.x));
    }

    void CollisionCheck()
    {
        Ray back = new Ray(pos_Tail.position, fish.Coordinate.Back);
        Ray bottom = new Ray(pos_Tail.position, fish.Coordinate.Down);

        float offset = 0.2f;

        //? 꼬리에서 아래쪽을 향하는 벡터
        Debug.DrawRay(pos_Tail.position, fish.Coordinate.Down * offset, Color.green);
        //? 꼬리에서 반대쪽방향을 향하는 벡터
        Debug.DrawRay(pos_Tail.position, fish.Coordinate.Back * offset, Color.red);
        //? 물고기 길이 확인용 선 / 이 선이 물고기의 배면에 닿아야함(혹은 배지느러미)
        Debug.DrawRay(pos_Tail.position, fish.Coordinate.Front * 3, Color.blue);

        RaycastHit hit_tail;
        RaycastHit hit_bottom;

        if (Physics.Raycast(back, out hit_tail, offset, LayerMask.GetMask("Ground")))
        {
            //Debug.Log(hit_tail.normal + "@꼬리뒷쪽");
            rig.AddForceAtPosition(hit_tail.normal * Time.deltaTime * addForce * 0.5f, hit_tail.point, ForceMode.VelocityChange);
        }
        if (Physics.Raycast(bottom, out hit_bottom, offset, LayerMask.GetMask("Ground")))
        {
            //Debug.Log(hit_bottom.normal + "@바닥쪽");
            rig.AddForceAtPosition(hit_bottom.normal * Time.deltaTime * addForce * 0.5f, hit_bottom.point, ForceMode.VelocityChange);
        }


        Ray body = new Ray(pos_Head.position, fish.Coordinate.Back);
        float length = (pos_Head.localPosition.x - pos_Tail.localPosition.x) * transform.localScale.x;
        Debug.DrawRay(pos_Head.position, fish.Coordinate.Back * length, Color.black);

        RaycastHit[] hit_body = Physics.RaycastAll(body, length, LayerMask.GetMask("Ground") | LayerMask.GetMask("Obstacle"));
        foreach (var col in hit_body)
        {
            //Debug.Log(col.normal + "@지형에박힘");
            rig.AddForceAtPosition(col.normal * Time.deltaTime * addForce, col.point, ForceMode.VelocityChange);
        }
    }

}
