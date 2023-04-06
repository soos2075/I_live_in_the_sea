using UnityEngine;

public class Fish : MonoBehaviour
{
    //? 필요한 스탯
    public float size;      //? 물고기의 크기

    public float moveSpeed; //? PlayerStat - 이동속도
    public float rotaSpeed; //? PlayerStat - 회전속도

    public float forceNormal = 100;  //? 기본 힘 전달량
    public float forceWeak = 10;     //? 특수상황 (물밖에 나갔거나 혹은 디버프)

    

    //? 플레이어블
    public enum Playerable
    {
        Neutrality, //? 중립
        Player,     //? 플레이어캐릭터 일때만, 지정은 PlayerController에서 해주고 이외에는 기본적으로 모두 중립인 상태.
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
        Initialize();
    }

    void Update()
    {

    }
    private void FixedUpdate()
    {
        switch (playerable)
        {
            case Playerable.Player:
                PlayerableUpdate();
                break;

            default:
                //? AI 업데이트
                FixedUpdate_NonPlayerable();
                break;
        }
    }

    void PlayerableUpdate()
    {
        RecoveryGage();
    }

    protected virtual void FixedUpdate_NonPlayerable()
    {
        MoveSelf();
    }



    protected virtual void Initialize()
    {
        Debug.Log($"초기화 안됨 {gameObject.name}");
    }
    protected void Initialize_Stat(float _size, float _moveSpd, float _roteSpd, float _forceN, float _forceW)
    {
        size = _size;
        moveSpeed = _moveSpd;
        rotaSpeed = _roteSpd;
        forceNormal = _forceN;
        forceWeak = _forceW;
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


    //? 물고기마다 가진 특수능력
    #region 특수능력 
    protected enum abilityType
    {
        Once,
        Keep,
    }
    protected abilityType a_type;
    public int abilityGage;
    public int abilityGageMax;
    protected int recoveryAmount;
    protected float recoveryFrequency;
    public void Ability()
    {
        switch (a_type)
        {
            case abilityType.Once:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Once Ability");
                    AbilityStart();
                }
                break;

            case abilityType.Keep:
                if (Input.GetKey(KeyCode.Space) && abilityGage > 0)
                {
                    Debug.Log("Keep Ability");
                    AbilityStart();
                }
                else if (Input.GetKeyUp(KeyCode.Space) || abilityGage <= 1)
                {
                    Debug.Log("Keep Ability Over");
                    AbilityOver();
                }
                break;
        }
    }
    protected virtual void AbilityStart()
    {
        Debug.Log("능력시작 초기화 안됨");
    }
    protected virtual void AbilityOver()
    {
        Debug.Log("능력종료 초기화 안됨");
    }

    protected void Initialize_Ability(abilityType type, int gage, int amount = 1, float frequency = 1.0f)
    {
        a_type = type;
        abilityGage = gage;
        abilityGageMax = gage;
        recoveryAmount = amount;
        recoveryFrequency = frequency;
    }

    float timer = 0;
    void RecoveryGage()
    {
        if (abilityGage < abilityGageMax && !Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;
            if (timer > recoveryFrequency)
            {
                timer -= recoveryFrequency;
                abilityGage += recoveryAmount;
            }
        }
    }
    #endregion
}
