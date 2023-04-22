using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkWhite : Fish
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
    public bool showRadiusGizmos;
    public bool showInteractRadius;



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
            Initialize_Ability(abilityType.Keep, 120, 4, 0.4f);
        }
        else
        {
            Settings_AI();
        }
    }

    protected override void VirtualFixedUpdate()
    {
        SetCoordinate(transform.right, -transform.right, transform.up, -transform.up);
    }

    protected override void PlayerableUpdate()
    {
        base.PlayerableUpdate();
        //SizeCheck();
    }
    protected override void FixedUpdate_NonPlayerable()
    {
        base.FixedUpdate_NonPlayerable();
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
