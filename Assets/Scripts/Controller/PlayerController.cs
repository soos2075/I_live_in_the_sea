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

        pos_Tail = transform.GetChild(0).GetChild(0);
        pos_Head = transform.GetChild(0).GetChild(1);
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
        if (fish.CheckOcean())
        {
            addForce = fish.ForceWeak;
        }
        else
        {
            addForce = fish.ForceNormal;
        }

        CollisionCheck();
        PlayerMoveKeyboard();
        RotateFinish();
        //TestFOV();
    }

    void TestFOV()
    {
        Vector3 dir = Quaternion.AngleAxis(30, transform.forward) * transform.right;
        Vector3 dir2 = Quaternion.AngleAxis(-30, transform.forward) * transform.right;

        Debug.DrawRay(transform.position, dir * 2, Color.red);
        Debug.DrawRay(transform.position, dir2 * 2, Color.green);
    }


    public float lastHorizontalInput = 0;
    void RotateFinish()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            lastHorizontalInput = Input.GetAxis("Horizontal");
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            return;
        }

        if (transform.eulerAngles.y % 180 < 1 || transform.eulerAngles.y % 180 > 179)
        {
            return;
        }


        //Debug.Log("들어오고잇음 분명");

        Vector3 onlyDir = Vector3.zero;

        if (lastHorizontalInput >= 0)
        {
            if (transform.eulerAngles.y < 180)
            {
                onlyDir = new Vector3(Vector3.right.x, transform.right.y, 0);
            }
            else if(transform.eulerAngles.y > 180)
            {
                onlyDir = new Vector3(Vector3.right.x, -transform.right.y, 0);
            }
        }
        else
        {
            if (transform.eulerAngles.y < 180)
            {
                onlyDir = new Vector3(Vector3.left.x, transform.right.y, 0);
            }
            else if (transform.eulerAngles.y > 180)
            {
                onlyDir = new Vector3(Vector3.left.x, -transform.right.y, 0);
            }
        }



        float angle_Z = 90 + Mathf.Atan2(onlyDir.x, onlyDir.y) * -Mathf.Rad2Deg;
        if (lastHorizontalInput > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle_Z),
                Time.deltaTime * fish.RotaSpeed * 0.5f);
        }
        else if (lastHorizontalInput < 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -angle_Z),
                Time.deltaTime * fish.RotaSpeed * 0.5f);
        }
    }


    void PlayerMoveKeyboard()
    {
        //? 방향
        Vector3 moveDir = Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");

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
                Time.deltaTime * fish.RotaSpeed * 0.5f);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -angle_Z),
                Time.deltaTime * fish.RotaSpeed * 0.5f);
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            //Debug.Log(transform.eulerAngles.y + "///" + angle_Z);
            float y = (transform.eulerAngles.y <= 180) 
                ? transform.eulerAngles.y 
                : 360 - transform.eulerAngles.y;

            if (y >= 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -angle_Z),
                Time.deltaTime * fish.RotaSpeed * 0.5f);
            }
            else if (y < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle_Z),
                Time.deltaTime * fish.RotaSpeed * 0.5f);
            }
        }
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
        float length = (pos_Head.position.x - pos_Tail.position.x);
        Debug.DrawRay(pos_Head.position, fish.Coordinate.Back * length, Color.black);

        RaycastHit[] hit_body = Physics.RaycastAll(body, length, LayerMask.GetMask("Ground") | LayerMask.GetMask("Obstacle"));
        foreach (var col in hit_body)
        {
            //Debug.Log(col.normal + "@지형에박힘");
            rig.AddForceAtPosition(col.normal * Time.deltaTime * addForce, col.point, ForceMode.VelocityChange);
        }
    }

}
