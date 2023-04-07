using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    //? 필요한 스탯
    public float size;      //? 물고기의 크기

    public float moveSpeed; //? PlayerStat - 이동속도
    public float rotaSpeed; //? PlayerStat - 회전속도

    public float forceNormal = 100;  //? 기본 힘 전달량
    public float forceWeak = 10;     //? 특수상황 (물밖에 나갔거나 혹은 디버프)

    public float flock_Radius;      //? 주변 군집객체 탐색 범위
    

    //? 플레이어블
    public enum Playerable
    {
        Neutrality, //? 중립
        Player,     //? 플레이어캐릭터 일때만, 지정은 PlayerController에서 해주고 이외에는 기본적으로 모두 중립인 상태.
        Hostile,    //? 적대적 - 보스같은게 있으면 쓰면 좋은데 아니면 삭제해도 무방
    }

    //? 현재상태 - 단체활동중인거랑은 별개의 개별상태임. 일단은 모든 개체가 잠재적 군체라고 가정하는게 좋을듯. 물론 분리수치가 높으면 떨어져나갈수도있고..
    public enum State
    {
        Wander,
        Sleep,
        Activity,
        Food,

        Chasing,
        Runaway,

        Attack,

        Dead,
    }

    public Playerable playerable;
    public State state;

    public bool isFlocking = false;

    void Start()
    {
        Initialize();
        StartCoroutine(FindNeighbourCoroutine());
    }

    protected virtual void Initialize()
    {
        //Initialize_Weight();
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


    bool CheckOcean()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 10, LayerMask.GetMask("Water")))
        {
            rig.useGravity = true;
            return true;
        }
        else
        {
            rig.useGravity = false;
            return false;
        }
    }


    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (CheckOcean())
        {
            return;
        }
        
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
        Vector3 ego = EgoVector() * 1;
        Vector3 cohesion = CalculateCohesionVector() * weight_Cohesion;
        Vector3 alignment = CalculateAlignmentVector() * weight_Alignment;
        Vector3 separation = CalculateSeparationVector() * weight_Separation;

        switch (state)
        {
            case State.Wander:
                moveDir = ego + cohesion + alignment + separation;

                break;
            case State.Sleep:
                break;
            case State.Activity:
                break;
            case State.Food:
                break;
            case State.Chasing:
                break;
            case State.Runaway:
                break;
            case State.Attack:
                break;
            case State.Dead:
                break;
            default:
                break;
        }

        MoveSelf();
    }

    protected Vector3 moveDir;
    protected float currentSpeed;
    protected Rigidbody rig;

    protected virtual void MoveSelf()
    {
        float lerpSpd = Random.Range(CalculateAlignmentSpeed(), currentSpeed);
        //Debug.Log(lerpSpd);

        Vector3 lerpDir = Vector3.Lerp(transform.right, moveDir, 0.5f);
        rig.AddForce(lerpDir.normalized * Time.deltaTime * lerpSpd * forceNormal);
        //rig.AddForce(moveDir.normalized * Time.deltaTime * currentSpeed * forceNormal);

        if (lerpDir.x > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + Mathf.Atan2(lerpDir.x, lerpDir.y) * -Mathf.Rad2Deg),
                Time.deltaTime * rotaSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + Mathf.Atan2(lerpDir.x, lerpDir.y) * -Mathf.Rad2Deg)),
                Time.deltaTime * rotaSpeed);
        }
    }
    
    protected virtual void MoveFlock()
    {

    }

    protected virtual void Runaway()
    {

    }

    protected virtual void Attack()
    {

    }

    //? 여기에 정렬 응집 분리 회피 찾기 추적 도착 도망 리더추격 헤메기   의 각 기능을 구현해놓고
    //? 각 가중치만 생선마다 다르게하고 움직임 전반과 로직은 똑같이 사용해도 될 것 같음.
    //? 즉 상태변화 + 가중치조정만으로 모든 물고기를 움직일 수 있게끔.
    #region Movement
    protected float weight_Cohesion;
    protected float weight_Alignment;
    protected float weight_Separation;

    protected void Initialize_Weight(float cohesion = 1, float alignment = 1, float separation = 1)
    {
        weight_Cohesion = cohesion;
        weight_Alignment = alignment;
        weight_Separation = separation;
    }


    public List<Fish> neighbours = new List<Fish>();
    Coroutine findNeighbourCoroutine;
    IEnumerator FindNeighbourCoroutine()
    {
        neighbours.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, flock_Radius, LayerMask.GetMask("Fish_Small"));
        for (int i = 0; i < colls.Length; i++)
        {
            neighbours.Add(colls[i].GetComponent<Fish>());
            if (i > 10)
            {
                break;
            }
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        findNeighbourCoroutine = StartCoroutine("FindNeighbourCoroutine");
    }
    protected Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 위치 더하기
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            // 이웃이 없으면 vector3.zero 반환
            return cohesionVec;
        }

        // 중심 위치로의 벡터 찾기
        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        cohesionVec.Normalize();
        return cohesionVec;
    }

    //? 정렬 - Alignment,
    protected Vector3 CalculateAlignmentVector()
    {
        Vector3 alignmentVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃들이 향하는 방향의 평균 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].moveDir;
            }
        }
        else
        {
            return alignmentVec;
        }

        alignmentVec /= neighbours.Count;
        alignmentVec.Normalize();
        return alignmentVec;
    }

    //? 분리 - Separation
    protected Vector3 CalculateSeparationVector()
    {
        Vector3 separationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃들을 피하는 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                separationVec += (transform.position - neighbours[i].transform.position);
            }
        }
        else
        {
            // 이웃이 없으면 vector.zero 반환
            return separationVec;
        }
        separationVec /= neighbours.Count;
        separationVec.Normalize();
        return separationVec;
    }

    //? 자아 - ego
    protected virtual Vector3 EgoVector()
    {
        return transform.forward;
    }
    //? 속도정렬 - Alignment_Speed
    protected float CalculateAlignmentSpeed()
    {
        float spd = currentSpeed;
        if (neighbours.Count > 0)
        {
            // 이웃들이 향하는 방향의 평균 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                spd += neighbours[i].currentSpeed;
            }
        }
        else
        {
            return spd;
        }

        spd /= neighbours.Count;
        return spd;
    }
    #endregion


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
