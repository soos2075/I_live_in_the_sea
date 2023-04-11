using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    //? 기본스탯
    public float Size { get; set; }      //? 물고기의 크기

    public float MoveSpeed { get; set; } //? PlayerStat - 이동속도
    public float RotaSpeed { get; set; } //? PlayerStat - 회전속도

    public float ForceNormal { get; set; } = 100;  //? 기본 힘 전달량
    public float ForceWeak { get; set; } = 10;     //? 특수상황 (물밖에 나갔거나 혹은 디버프)
  

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
        Non,
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
    public State StateFish { 
        get { return state; } 
        set { 
            state = value;
            switch (state)
            {
                case State.Wander:
                    currentSpeed = ranSpd * 0.5f;
                    break;
                case State.Sleep:
                    currentSpeed = ranSpd * 0.1f;
                    break;
                case State.Activity:
                    currentSpeed = ranSpd * 1.0f;
                    break;
                case State.Food:
                    currentSpeed = MoveSpeed;
                    break;
                case State.Chasing:
                    currentSpeed = MoveSpeed;
                    break;
                case State.Runaway:
                    currentSpeed = MoveSpeed;
                    rig.AddForce(currentDir.normalized * Time.deltaTime * MoveSpeed * ForceNormal * 10);
                    break;
                case State.Attack:
                    break;
                case State.Dead:
                    break;
            }
        } 
    }

    public bool isFlocking = false;

    void Start()
    {
        Initialize();
        forward = transform.right.normalized;

        if (playerable == Playerable.Player)
        {

        }
        else
        {
            ranAngle = Random.Range(-15, 15);

            SetRandomValue();
            randomValueCor = StartCoroutine(RandomValueRepeater());

            findNeighbourCoroutine = StartCoroutine(FindNeighbourCoroutine());
            searchPredatorCoroutine = StartCoroutine(SearchPredatorCoroutine());
            searchFoodCoroutine = StartCoroutine(SearchFoodCoroutine());
        }

    }

    protected virtual void Initialize()
    {
        Debug.Log($"필수초기화 안됨 {gameObject.name}");
    }
    protected void Initialize_Stat(float _size, float _moveSpd, float _roteSpd, float _forceN, float _forceW)
    {
        Size = _size;
        MoveSpeed = _moveSpd;
        RotaSpeed = _roteSpd;
        ForceNormal = _forceN;
        ForceWeak = _forceW;
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
                StateCheck();
                FixedUpdate_NonPlayerable();
                break;
        }
    }


    void PlayerableUpdate()
    {
        RecoveryGage();
    }

    #region StateCheck
    public int HungerGage = 100;
    public int ActivityGage = 100;
    private float hungerCount;
    private float activityCount;
    void StateCheck()
    {
        if (predator.Count > 0)
        {
            if (StateFish == State.Runaway) return;
            StateFish = State.Runaway;
        }
        else if (HungerGage < 90 && HungerGage < 30 || food.Count > 3)
        {
            if (StateFish == State.Food) return;
            StateFish = State.Food;
        }
        //else if (ActivityGage > 30 && neighbours.Count > FlockCount * 0.5f)
        //{
        //    if (StateFish == State.Activity) return;
        //    StateFish = State.Activity;
        //}
        else if (ActivityGage <= 30)
        {
            if (StateFish == State.Sleep) return;
            StateFish = State.Sleep;
        }
        else
        {
            if (StateFish == State.Wander) return;
            StateFish = State.Wander;
        }
    }

    void Hunger()
    {
        hungerCount += Time.deltaTime;
        if (hungerCount > 5)
        {
            hungerCount = 0;
            HungerGage--;
        }
    }
    void Activity()
    {
        activityCount += Time.deltaTime;
        if (activityCount > 5)
        {
            activityCount = 0;
            ActivityGage--;
        }
    }
    #endregion

    protected virtual void FixedUpdate_NonPlayerable()
    {
        Hunger();

        Vector3 ego = SetEgoVector() * Weight_Ego * (currentSpeed * 0.5f);
        Vector3 cohesion = CalculateCohesionVector() * Weight_Cohesion;
        Vector3 alignment = CalculateAlignmentVector() * Weight_Alignment;
        Vector3 separation = CalculateSeparationVector() * Weight_Separation;
        Vector3 leader = CalculateLeaderVector() * Weight_Leader;
        Vector3 food = CalculateFood() * 2;
        Vector3 predator = CalculatePredator() * 10;
        AvodeGround();
        StayBoundary();

        switch (StateFish)
        {
            case State.Wander:
                currentDir = (ego * 1.5f) + cohesion + alignment + separation + food ;
                break;

            case State.Sleep:
                currentDir = ego + SetDestination(Vector3.down);
                break;

            case State.Activity:
                Activity();
                currentDir = ego + cohesion + alignment + separation + leader;
                break;

            case State.Food:
                currentDir = food;
                break;

            case State.Chasing:
                Activity();
                break;

            case State.Runaway:
                Activity();
                currentDir = predator;
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

    #region protected Field
    protected Vector3 currentDir;
    protected float currentSpeed;
    protected Rigidbody rig;
    protected int ranAngle;
    protected Vector3 forward;
    protected float lastRotateCount;

    protected Vector3 ranDir;
    protected float ranSpd;

    [SerializeField] Vector3 lerpDir;
    [SerializeField] float lerpSpd;
    #endregion

    protected void MoveSelf()
    {
        lastRotateCount += Time.deltaTime;
        //? 역방향 회전시 순간부스터
        if (Mathf.Abs(forward.x) > 0.25f && Mathf.Abs(currentDir.normalized.x) > 0.25f && Mathf.Abs(forward.x + currentDir.normalized.x) < 0.5f)
        {
            ranAngle = Random.Range(-15, 15);
            forward = currentDir.normalized;
            currentSpeed = Mathf.Lerp(currentSpeed, CalculateAlignmentSpeed(), Random.Range(0.3f,0.9f));
            rig.AddForce(currentDir.normalized * Time.deltaTime * MoveSpeed * ForceNormal * 10);
            lastRotateCount = 0;
        }

        lerpSpd = Mathf.Lerp(currentSpeed, CalculateAlignmentSpeed(), 0.5f);
        //float lerpSpd = CalculateAlignmentSpeed();

        lerpDir = Vector3.Lerp(transform.right.normalized, currentDir.normalized, 0.35f);
        //Vector3 lerpDir = currentDir;

       rig.AddForce(lerpDir.normalized * Time.deltaTime * lerpSpd * ForceNormal);
        //rig.AddForce(moveDir.normalized * Time.deltaTime * currentSpeed * forceNormal);

        float degree = Mathf.Atan2(lerpDir.x, lerpDir.y) * -Mathf.Rad2Deg;
        if (lerpSpd > 1.0f) //? 속도가 빠를땐 방향에 따른 부드러운 회전
        {
            if (lerpDir.x > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 + degree),
                    Time.deltaTime * RotaSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -(90 + degree)),
                    Time.deltaTime * RotaSpeed);
            }
        }
        else //? 속도가 느릴땐 방향고정 & 수직상하이동
        {
            if (currentDir.x > 0) 
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, ranAngle),
                    Time.deltaTime * RotaSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(180, 0, -180 + ranAngle),
                    Time.deltaTime * RotaSpeed);
            }
        }
    }


    //? 여기에 정렬 응집 분리 회피 찾기 추적 도착 도망 리더추격 헤메기   의 각 기능을 구현해놓고
    //? 각 가중치만 생선마다 다르게하고 움직임 전반과 로직은 똑같이 사용해도 될 것 같음.
    //? 즉 상태변화 + 가중치조정만으로 모든 물고기를 움직일 수 있게끔.
    #region Movement

    #region Property
    protected float Weight_Cohesion { get; set; }
    protected float Weight_Alignment { get; set; }
    protected float Weight_Separation { get; set; }
    protected float Weight_Ego { get; set; }
    protected float Weight_Leader { get; set; }
    protected float Weight_Food { get; set; }
    protected float Weight_Predator { get; set; }


    protected void Initialize_Weight(float cohesion = 1, float alignment = 1, float separation = 1, float ego = 1, float leader = 1, 
        float food = 1, float predator = 1)
    {
        Weight_Cohesion = cohesion;
        Weight_Alignment = alignment;
        Weight_Separation = separation;
        Weight_Ego = ego;
        Weight_Leader = leader;
        Weight_Food = food;
        Weight_Predator = predator;
    }


    public List<Fish> neighbours = new List<Fish>();
    public List<Fish> food = new List<Fish>();
    public List<Fish> predator = new List<Fish>();

    public float RandomResetCount { get; set; }
    protected int FlockCount { get; set; }
    protected float FlockRadius { get; set; }
    protected float SearchRadius { get; set; }
    protected float SearchAngle { get; set; }

    protected int FlockLayer { get; set; }
    protected int FoodLayer { get; set; }
    protected int PredatorLayer { get; set; }
    protected int BoundaryLayer { get; set; }
    #endregion


    //? 응집 - Cohesion
    protected Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 1)
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
        if (neighbours.Count > 1)
        {
            // 이웃들이 향하는 방향의 평균 방향으로 이동
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].currentDir;
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
        if (neighbours.Count > 1)
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

    //? 대장따르기 - Leader
    protected Vector3 CalculateLeaderVector()
    {
        Vector3 vec = currentDir;
        float spd = currentSpeed;
        if (neighbours.Count > 1)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i].currentSpeed > spd)
                {
                    vec = neighbours[i].currentDir;
                    spd = neighbours[i].currentSpeed;
                }
            }
        }
        vec.Normalize();
        return vec;
    }

    //? 목적지 - Destination
    protected Vector3 SetDestination(Vector3 target)
    {
        return target;
    }

    //? 자아 - ego
    protected Vector3 SetEgoVector()
    {
        return ranDir.normalized;
    }

    //? 랜덤리셋 - RandomReset
    protected void SetRandomValue()
    {
        ranDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), 0);
        ranSpd = Random.Range(MoveSpeed * 0.5f, MoveSpeed);
    }

    //? 먹이탐색 - Exploration_Food
    protected Vector3 CalculateFood()
    {
        Vector3 vec = Vector3.zero;
        if (food.Count > 0)
        {
            vec += food[0].transform.position - transform.position;
            for (int i = 0; i < food.Count; i++)
            {
                if (vec.magnitude > (food[i].transform.position - transform.position).magnitude)
                {
                    vec += food[i].transform.position - transform.position;
                }
            }
        }
        else
        {
            return vec;
        }
        vec.Normalize();
        return vec;
    }

    //? 포식자탐색 - Exploration_Predator
    protected Vector3 CalculatePredator()
    {
        Vector3 vec = Vector3.zero;
        if (predator.Count > 0)
        {
            vec = transform.position - predator[0].transform.position;
        }
        else
        {
            return vec;
        }
        vec.Normalize();
        return vec;
    }

    //? 지형회피 - Avode
    protected void AvodeGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 1, LayerMask.GetMask("Ground")))
        {
            ranDir = new Vector3(hit.normal.x, hit.normal.y, 0);
            rig.AddForce(ranDir.normalized * Time.deltaTime * MoveSpeed * ForceNormal * 10);
        }
        else if (Physics.Raycast(transform.position, transform.right, out hit, 1, LayerMask.GetMask("Water")))
        {
            ranDir = new Vector3(transform.right.x, -transform.right.y, 0);
        }
    }

    //? 지역머물기 - Stay
    protected Boundary.Data boundaryData;
    protected void StayBoundary()
    {
        if (boundaryData != null)
        {
            if (!Physics.CheckSphere(transform.position, Size, BoundaryLayer))
            {
                float offset = boundaryData.radius * 0.5f;
                ranDir = (boundaryData.pos + new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), 0)) - transform.position;
            }
        }
    }


    //? 속도정렬 - Alignment_Speed
    protected float CalculateAlignmentSpeed()
    {
        float spd = 0;
        if (neighbours.Count > 1)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                spd += neighbours[i].currentSpeed;
            }
        }
        else
        {
            return currentSpeed;
        }
        spd /= neighbours.Count;
        return spd;
    }
    #endregion



    //? 코루틴
    #region Coroutine
    //? 군체찾기
    Coroutine findNeighbourCoroutine;
    IEnumerator FindNeighbourCoroutine()
    {
        neighbours.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, FlockRadius, FlockLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            neighbours.Add(colls[i].GetComponent<Fish>());
            if (i > FlockCount)
            {
                break;
            }
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        findNeighbourCoroutine = StartCoroutine("FindNeighbourCoroutine");
    }

    //? 먹이찾기
    Coroutine searchFoodCoroutine;
    IEnumerator SearchFoodCoroutine()
    {
        food.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, SearchRadius, FoodLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            food.Add(colls[i].GetComponent<Fish>());
            if (i > 5)
            {
                break;
            }
        }
        yield return new WaitForSeconds(1);
        searchFoodCoroutine = StartCoroutine(SearchFoodCoroutine());
    }

    //? 포식자찾기
    Coroutine searchPredatorCoroutine;
    IEnumerator SearchPredatorCoroutine()
    {
        predator.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, SearchRadius, PredatorLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            predator.Add(colls[i].GetComponent<Fish>());
            if (i > 1)
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);
        searchPredatorCoroutine = StartCoroutine(SearchPredatorCoroutine());
    }

    //? 랜덤값 리셋
    Coroutine randomValueCor;
    IEnumerator RandomValueRepeater()
    {
        yield return new WaitUntil(() => lastRotateCount > RandomResetCount);
        SetRandomValue();
        lastRotateCount = 0;
        randomValueCor = StartCoroutine(RandomValueRepeater());
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
