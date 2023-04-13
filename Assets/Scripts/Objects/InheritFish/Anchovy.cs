using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchovy : Fish
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



    void Settings_Default()
    {
        rig = GetComponent<Rigidbody>();
        Initialize_Stat(_size, _moveSpeed, _rotaSpeed, _forceNormal, _forceWeak);

        FlockLayer = LayerMask.GetMask("Fish_Small") | LayerMask.GetMask("Fish_Middle");
        FoodLayer = LayerMask.GetMask("Zooplankton") | LayerMask.GetMask("Phytoplankton");
        PredatorLayer = LayerMask.GetMask("Predator_Small") | LayerMask.GetMask("Predator_Middle") | LayerMask.GetMask("Predator_Large");
    }
    void Settings_AI()
    {
        Initialize_Weight(_cohesion, _alignment, _separation, _ego, _leader);

        RandomResetCount = _randomResetCount;
        FlockCount = _flockCount;
        FlockRadius = _flockRadius;
        SearchRadius = _predatorRadius;
        SearchAngle = _predatorAngle;

        SetBoundary();
    }
    void SetBoundary()
    {
        BoundaryLayer = LayerMask.GetMask("Boundary");
        boundaryData = FindObjectOfType<Boundary>().GetBoundaryData();
    }


    protected override void Initialize()
    {
        Settings_Default();

        if (playerable == Playerable.Player)
        {
            Initialize_Ability(abilityType.Keep, 120, 4, 0.4f);
        }
        else
        {
            Settings_AI();
        }
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
    }

    protected override void FixedUpdate_NonPlayerable()
    {
        //_ego = test.egoWeight;
        //_cohesion = test.cohesionWeight;
        //_alignment = test.alignmentWeight;
        //_separation = test.separationWeight;

        CollisionInteract();

        base.FixedUpdate_NonPlayerable();
    }

    void CollisionInteract()
    {
        Debug.DrawRay(transform.position, ranDir.normalized * 1);
        Debug.DrawRay(transform.position, transform.right * 1, Color.blue);
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
