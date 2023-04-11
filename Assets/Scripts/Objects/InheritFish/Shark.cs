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
        FoodLayer = LayerMask.GetMask("Fish_Small") | LayerMask.GetMask("Fish_Middle");
        PredatorLayer = 0;

        SetBoundary();
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
