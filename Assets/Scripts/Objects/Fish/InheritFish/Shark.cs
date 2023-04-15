using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : Fish
{
    [Space(10)]
    [Header("Stat Settings")]
    public float _size;
    public float _moveSpeed;
    public float _rotaSpeed;
    public float _forceNormal;
    public float _forceWeak;

    [Space(10)]
    [Header("Weight Settings")]
    [Range(0, 5)] public float _ego;
    [Range(0, 5)] public float _cohesion;
    [Range(0, 5)] public float _alignment;
    [Range(0, 5)] public float _separation;
    [Range(0, 5)] public float _leader;

    [Space(10)]
    [Header("Etc Settings")]
    public float _randomResetCount;
    public int _flockCount;
    public float _flockRadius;
    public float _predatorRadius;
    public float _predatorAngle;
    public bool showRadiusGizmos;



    void Settings()
    {
        Initialize_Stat(_size, _moveSpeed, _rotaSpeed, _forceNormal, _forceWeak);
        Initialize_Weight(_cohesion, _alignment, _separation, _ego, _leader);

        RandomResetCount = _randomResetCount;
        FlockCount = _flockCount;
        FlockRadius = _flockRadius;
        SearchRadius = _predatorRadius;
        SearchAngle = _predatorAngle;

        FlockLayer = LayerMask.GetMask("Predator_Large");
        PredatorLayer = GetComponent<Prey>().predatorLayer;
        PreyLayer = GetComponent<Predator>().preyLayer;
        //PreyLayer = LayerMask.GetMask("Fish_Small") | LayerMask.GetMask("Fish_Middle");
        //PredatorLayer = 0;

        //SetBoundary();
    }
    void SetBoundary()
    {
        BoundaryLayer = LayerMask.GetMask("Boundary");
        boundaryData = FindObjectOfType<Boundary>().GetBoundaryData();
    }


    protected override void Initialize()
    {
        Settings();

        rig = GetComponent<Rigidbody>();


        if (playerable == Playerable.Player)
        {
            Initialize_Ability(abilityType.Keep, 120, 4, 0.4f);
        }
        else
        {

        }

        GetPos();
    }

    Transform pos_Tail;
    Transform pos_Head;
    void GetPos()
    {
        pos_Tail = transform.GetChild(0).GetChild(0);
        pos_Head = transform.GetChild(0).GetChild(1);
    }
    protected void SizeCheck()
    {
        Ray back = new Ray(pos_Tail.position, Coordinate.Back);
        Ray bottom = new Ray(pos_Tail.position, Coordinate.Down);

        float offset = 0.2f;

        //? 꼬리에서 아래쪽을 향하는 벡터
        Debug.DrawRay(pos_Tail.position, Coordinate.Down * offset, Color.green);
        //? 꼬리에서 반대쪽방향을 향하는 벡터
        Debug.DrawRay(pos_Tail.position, Coordinate.Back * offset, Color.red);

        //? 물고기 가로길이 확인용 / 이 선이 물고기의 배면에 닿아야함(혹은 배지느러미)
        Debug.DrawRay(pos_Tail.position, Coordinate.Front * 20, Color.blue);
        //? 물고기 세로길이 확인용 선 / 이 선이 물고기의 꼬리끝에 닿아야함
        Debug.DrawRay(pos_Tail.position, Coordinate.Up * 10, Color.white);
    }


    #region Player Ability
    protected override void AbilityStart()
    {
        MoveSpeed = 15f;
        abilityGage--;
    }

    protected override void AbilityOver()
    {
        MoveSpeed = 5;
    }
    #endregion


    protected override void VirtualFixedUpdate()
    {
        SetCoordinate(transform.right, -transform.right, transform.up, -transform.up);
    }

    protected override void PlayerableUpdate()
    {
        base.PlayerableUpdate();
        SizeCheck();
    }
    protected override void FixedUpdate_NonPlayerable()
    {
        base.FixedUpdate_NonPlayerable();
    }


    private void OnDrawGizmos()
    {
        if (showRadiusGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, _flockRadius);
            Gizmos.DrawWireSphere(transform.position, _predatorRadius);
        }
    }
}
