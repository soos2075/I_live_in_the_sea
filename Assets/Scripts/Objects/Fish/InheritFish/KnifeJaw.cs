using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeJaw : Fish
{
    [Space(10)]
    [Header("Default Settings")]
    public float _size;
    public float _moveSpeed;
    public float _rotaSpeed;
    public float _forceNormal;
    public float _forceWeak;
    public float _interactRadius;

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


    void Settings_Default()
    {
        rig = GetComponent<Rigidbody>();
        Initialize_Stat(_size, _moveSpeed, _rotaSpeed, _forceNormal, _forceWeak);

        FlockLayer = LayerMask.GetMask("Fish_Small") | LayerMask.GetMask("Fish_Middle");
        PredatorLayer = GetComponent<Prey>().predatorLayer;
        PreyLayer = GetComponent<Predator>().preyLayer;
        AreaLayer = GetComponent<Area>().areaLayer;

        InteractRadius = _interactRadius;
    }
    void Settings_AI()
    {
        Initialize_Weight(_cohesion, _alignment, _separation, _ego, _leader);

        RandomResetCount = _randomResetCount;
        FlockCount = _flockCount;
        FlockRadius = _flockRadius;
        SearchRadius = _predatorRadius;
        SearchFOV = _predatorAngle;
    }



    protected override void Initialize()
    {
        Settings_Default();

        if (playerable == Playerable.Player)
        {
            //Initialize_Ability(abilityType.Keep, 120, 4, 0.4f);
        }
        else
        {
            Settings_AI();
        }
    }

    protected override void VirtualFixedUpdate()
    {
        SetCoordinate(transform.right, -transform.right, transform.up, -transform.up);
        //SizeCheck();
        //CollisionInteract();
    }
    protected override void PlayerableUpdate()
    {
        base.PlayerableUpdate();
    }

    protected override void FixedUpdate_NonPlayerable()
    {
        base.FixedUpdate_NonPlayerable();
    }

    #region DebugRay - 전방확인 - 향하는 방향, 가려고 하는 방향 / 후방확인 - 꼬리 아래 지형체크, 전체 사이즈 체크
    void SizeCheck() //? 사이즈 체크
    {
        Ray back = new Ray(Pos_Tail.position, Coordinate.Back);
        Ray bottom = new Ray(Pos_Tail.position, Coordinate.Down);

        float offset = 0.2f;

        //? 꼬리에서 아래쪽을 향하는 벡터
        Debug.DrawRay(Pos_Tail.position, Coordinate.Down * offset, Color.green);
        //? 꼬리에서 반대쪽방향을 향하는 벡터
        Debug.DrawRay(Pos_Tail.position, Coordinate.Back * offset, Color.red);

        //? 물고기 가로길이 확인용 / 이 선이 물고기의 배면에 닿아야함(혹은 배지느러미)
        Debug.DrawRay(Pos_Tail.position, Coordinate.Front * Size, Color.blue);
        //? 물고기 세로길이 확인용 선 / 이 선이 물고기의 꼬리끝에 닿아야함
        Debug.DrawRay(Pos_Tail.position, Coordinate.Up * (Size), Color.white);

        //? 체장 확인
        Ray body = new Ray(Pos_Head.position, Coordinate.Back);
        float length = (Pos_Head.position.x - Pos_Tail.position.x);
        Debug.DrawRay(Pos_Head.position, Coordinate.Back * length, Color.black);
    }
    void CollisionInteract() //? 방향체크
    {
        Debug.DrawRay(Pos_Head.transform.position, ranDir.normalized * InteractRadius, Color.blue);
        Debug.DrawRay(Pos_Head.transform.position, transform.right * InteractRadius, Color.green);
    }
    #endregion

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




    public bool showRadiusGizmos;
    public bool showInteractRadius;
    private void OnDrawGizmos()
    {
        if (showRadiusGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _flockRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _predatorRadius);
        }

        if (showInteractRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.GetChild(0).GetChild(1).position, _interactRadius);
        }
    }


}
