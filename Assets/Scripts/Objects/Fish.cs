using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    //? 필요한 스탯
    public float moveSpeed; //? PlayerStat - 이동속도
    public float rotaSpeed; //? PlayerStat - 회전속도

    public float size;

    //? 플레이어블
    public enum Playerable
    {
        Player,
        Neutrality, //? 중립
        Hostile,    //? 적대적

    }
    //? 현재상태
    public enum State
    {
        Idle,
        Chasing,
        Runaway,
    }

    public Playerable playerable;
    public State state;



    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected virtual void MoveSelf()
    {

    }

    protected virtual void Runaway()
    {

    }

    protected virtual void Attack()
    {

    }

}
