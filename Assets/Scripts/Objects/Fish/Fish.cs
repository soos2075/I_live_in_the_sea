using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Predator), typeof(Prey), typeof(Area))]
public class Fish : MonoBehaviour
{
    #region 기본스탯
    public float Size { get; set; }      //? 물고기의 크기

    public float MoveSpeed { get; set; } //? PlayerStat - 이동속도
    public float RotaSpeed { get; set; } //? PlayerStat - 회전속도

    public float ForceNormal { get; set; } = 100;  //? 기본 힘 전달량
    public float ForceWeak { get; set; } = 10;     //? 특수상황 (물밖에 나갔거나 혹은 디버프)
    #endregion

    #region 플레이어블 선택
    public enum Playerable
    {
        Neutrality, //? 중립
        Player,     //? 플레이어캐릭터 일때만, 지정은 PlayerController에서 해주고 이외에는 기본적으로 모두 중립인 상태.
        Hostile,    //? 적대적 - 보스같은게 있으면 쓰면 좋은데 아니면 삭제해도 무방
    }
    #endregion

    #region State
    public enum State
    {
        Non,
        Wander,
        Sleep,
        Activity,
        SlowFood,

        FastFood,
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
                case State.SlowFood:
                    currentSpeed = ranSpd * 0.8f;
                    break;
                case State.FastFood:
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
        else if (HungerGage < 90 && food.Count > 3)
        {
            if (StateFish == State.SlowFood) return;
            StateFish = State.SlowFood;
        }
        else if(HungerGage <= 30)
        {
            if (StateFish == State.FastFood) return;
            StateFish = State.FastFood;
        }
        else if (ActivityGage > 30 && neighbours.Count > FlockCount * 0.5f)
        {
            if (StateFish == State.Activity) return;
            StateFish = State.Activity;
        }
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

    #region 좌표계통일
    public struct CoordinateUnification
    {
        public Vector3 Front;
        public Vector3 Back;
        public Vector3 Up;
        public Vector3 Down;
    }

    public CoordinateUnification Coordinate;

    protected void SetCoordinate(Vector3 front, Vector3 back, Vector3 up, Vector3 down)
    {
        Coordinate.Front = front;
        Coordinate.Back = back;
        Coordinate.Up = up;
        Coordinate.Down = down;
    }
    #endregion


    public Transform Pos_Head { get; set; }
    public Transform Pos_Tail { get; set; }
    public Transform Pos_Gravity { get; set; }

    void Start()
    {
        Pos_Head = transform.GetChild(0).GetChild(1);
        Pos_Tail = transform.GetChild(0).GetChild(0);
        Transform qwer = Util.FindChild<Transform>(gameObject, "Pos_Gravity", true);
        Pos_Gravity = qwer == null ? transform : qwer;

        Initialize();

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
            StayBoundary();
        }

        transform.localScale = (Vector3.one * Size * 0.1f);
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


    public bool CheckOcean()
    {
        if (Physics.Raycast(Pos_Gravity.transform.position, Vector3.down, 100, LayerMask.GetMask("Water")))
        {
            //rig.useGravity = true;
            return true;
        }
        else
        {
            //rig.useGravity = false;
            return false;
        }
    }


    //void Update()
    //{
        
    //}
    private void FixedUpdate()
    {
        //? 중력량 테스트
        //rig.AddForce(Vector3.down * Time.deltaTime * 1500, ForceMode.Acceleration);

        VirtualFixedUpdate();

        if (CheckOcean())
        {
            rig.AddForce(Vector3.down * Time.deltaTime * 1500, ForceMode.Acceleration);
        }
        
        switch (playerable)
        {
            case Playerable.Player:
                PlayerableUpdate();
                break;

            case Playerable.Neutrality:
                //? AI 업데이트
                StateCheck();
                FixedUpdate_NonPlayerable();
                break;

            default:
                break;
        }
    }

    protected virtual void VirtualFixedUpdate()
    {

    }

    protected virtual void PlayerableUpdate()
    {
        RecoveryGage();
    }

    protected virtual void FixedUpdate_NonPlayerable()
    {
        Hunger();

        Vector3 ego = SetEgoVector() * Weight_Ego;// * (currentSpeed * 0.5f);
        Vector3 cohesion = CalculateCohesionVector() * Weight_Cohesion;

        Vector3 alignment = CalculateAlignmentVector() * Weight_Alignment;
        Vector3 alignmentDirection = CalculateAlignmentDirectionVector() * Weight_Alignment * 0.5f;

        Vector3 separation = CalculateSeparationVector() * Weight_Separation;
        Vector3 leader = CalculateLeaderVector() * Weight_Leader;
        Vector3 food = CalculateFood();
        Vector3 predator = CalculatePredator() * 10;
        Vector3 avoidGround = CalculateAvoidGroundVector() * 10;

        WaterBlow();

        switch (StateFish)
        {
            case State.Wander:
                currentDir = (ego * 1.25f) + cohesion + alignment + separation + (food * 0.5f) + avoidGround;
                break;

            case State.Sleep:
                currentDir = ego + SetDestination(Vector3.down);
                break;

            case State.Activity:
                Activity();
                currentDir = (ego * 0.75f) + cohesion + alignment + alignmentDirection + separation + leader + avoidGround;
                break;

            case State.SlowFood:
                currentDir = (ego * 0.5f) + (separation * 2.5f) + (food * 2.5f);
                break;

            case State.FastFood:
                currentDir = (ego * 0.5f) + (separation * 2.5f) + (food * 2.5f);
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
    protected Vector3 currentDir { get; set; }
    protected float currentSpeed;
    protected Rigidbody rig;
    protected int ranAngle;
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
        if (Mathf.Abs(Coordinate.Front.x) > 0.25f && Mathf.Abs(currentDir.normalized.x) > 0.25f && Mathf.Abs(Coordinate.Front.x + currentDir.normalized.x) < 0.5f)
        {
            ranAngle = Random.Range(-15, 15);
            currentSpeed = Mathf.Lerp(currentSpeed, CalculateAlignmentSpeed(), Random.Range(0.3f,0.9f));
            rig.AddForce(currentDir.normalized * Time.deltaTime * MoveSpeed * 2, ForceMode.Impulse);
            lastRotateCount = 0;
        }

        lerpSpd = Mathf.Lerp(currentSpeed, CalculateAlignmentSpeed(), 0.5f);
        //float lerpSpd = CalculateAlignmentSpeed();

        lerpDir = Vector3.Lerp(Coordinate.Front.normalized, currentDir.normalized, 0.4f);
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
    public List<Prey> food = new List<Prey>();
    public List<Predator> predator = new List<Predator>();

    public float RandomResetCount { get; set; }
    protected int FlockCount { get; set; }
    protected float FlockRadius { get; set; }
    protected float SearchRadius { get; set; }
    protected float SearchFOV { get; set; }

    protected int FlockLayer { get; set; }
    public int PreyLayer { get; protected set; }
    public int PredatorLayer { get; protected set; }
    protected int AreaLayer { get; set; }

    public float InteractRadius { get; set; }
    #endregion


    //? 응집 - Cohesion
    protected Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (neighbours.Count > 1)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
            }
        }
        else
        {
            return cohesionVec;
        }

        cohesionVec /= neighbours.Count;
        cohesionVec -= transform.position;
        cohesionVec.Normalize();
        return cohesionVec;
    }

    //? 전방 정렬 - Alignment Foward (개체들이 보고있는 방향의 평균)
    protected Vector3 CalculateAlignmentVector()
    {
        Vector3 alignmentVec = Vector3.zero;
        if (neighbours.Count > 1)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                alignmentVec += neighbours[i].Coordinate.Front;
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

    //? 방향 정렬 - Alignment Direction (개체들이 가고싶은 방향(EgoVector)의 평균)
    protected Vector3 CalculateAlignmentDirectionVector()
    {
        Vector3 alignmentVec = Vector3.zero;
        if (neighbours.Count > 1)
        {
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
            for (int i = 0; i < food.Count; i++)
            {
                vec += food[i].transform.position;
                //? 만약 먹이가 움직이지 않는 산호와 같은 종류라면
                if (food[i].GetComponent<Fish>() == null)
                {
                    Vector3 targetVec = food[i].transform.position - transform.position;
                    return targetVec.normalized;
                }
            }
        }
        else
        {
            return vec;
        }

        vec /= food.Count;
        vec -= transform.position;
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
    protected Vector3 CalculateAvoidGroundVector()
    {
        Vector3 vec = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(Pos_Head.transform.position, Coordinate.Front, out hit, InteractRadius * 2, LayerMask.GetMask("Ground"))||
            Physics.Raycast(Pos_Head.transform.position, Quaternion.AngleAxis(SearchFOV, transform.forward) * Coordinate.Front, out hit, InteractRadius * 2, LayerMask.GetMask("Ground"))||
            Physics.Raycast(Pos_Head.transform.position, Quaternion.AngleAxis(-SearchFOV, transform.forward) * Coordinate.Front, out hit, InteractRadius * 2, LayerMask.GetMask("Ground")))
        {
            vec = new Vector3(hit.normal.x, hit.normal.y, 0);
            ranDir = vec + Vector3.Reflect(Coordinate.Front, hit.normal);
            rig.AddForce(ranDir.normalized * Time.deltaTime * 100, ForceMode.Impulse);
            neighbours.Clear();
        }
        vec.Normalize();
        return vec;
    }


    //? 물넘기 - WaterBlow
    protected void WaterBlow()
    {
        //Debug.DrawRay(Pos_Head.transform.position, Coordinate.Front * InteractRadius, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(Pos_Head.transform.position, Coordinate.Front, out hit, InteractRadius, LayerMask.GetMask("Water")))
        {
            if (ContactSurface == null && !CheckOcean())
            {
                ContactSurface = StartCoroutine(ReturnTheSea(Vector3.Reflect(Coordinate.Front, hit.normal)));
            }
        }

    }

    Coroutine ContactSurface;
    Coroutine Co_SimpleTimer;
    bool isTimeOver;
    IEnumerator ReturnTheSea(Vector3 dir)
    {
        Co_SimpleTimer = StartCoroutine(SimpleTimer(3));
        yield return new WaitUntil(() => (CheckOcean()) || isTimeOver);
        if (!isTimeOver)
        {
            StopCoroutine(Co_SimpleTimer);
            ranDir = dir;
            rig.AddForce(Coordinate.Front.normalized * Time.deltaTime * 200, ForceMode.VelocityChange);
        }
        ContactSurface = null;
    }
    IEnumerator SimpleTimer(float timer)
    {
        isTimeOver = false;
        float temp = 0;
        while (temp < timer)
        {
            temp += Time.deltaTime;
            yield return null;
        }
        isTimeOver = true;
    }


    //? 지역머물기 - Stay
    protected void StayBoundary()
    {
        returnBoundary = StartCoroutine(ReturnBoundary());
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
            if (i >= FlockCount)
            {
                break;
            }
            neighbours.Add(colls[i].gameObject.GetComponentOrParent<Fish>());
        }
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        findNeighbourCoroutine = StartCoroutine("FindNeighbourCoroutine");
    }

    //? 먹이찾기
    Coroutine searchFoodCoroutine;
    IEnumerator SearchFoodCoroutine()
    {
        food.Clear();
        Collider[] colls = Physics.OverlapSphere(transform.position, SearchRadius, PreyLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            food.Add(colls[i].gameObject.GetComponentOrParent<Prey>());
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
            predator.Add(colls[i].gameObject.GetComponentOrParent<Predator>());
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
        yield return new WaitUntil(() => lastRotateCount > Random.Range(RandomResetCount * 0.5f, RandomResetCount));
        SetRandomValue();
        lastRotateCount = 0;
        randomValueCor = StartCoroutine(RandomValueRepeater());
    }

    Coroutine returnBoundary;
    //? Boundary로 돌아가기
    IEnumerator ReturnBoundary()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        if (!Physics.CheckSphere(transform.position, 1, AreaLayer))
        {
            var data = GameManager.Area.GetCloseBoundary(AreaLayer, transform.position).GetBoundaryData();
            float offset = data.radius * 0.5f;
            ranDir = (data.centerPos + new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), 0)) - transform.position;
            lastRotateCount = 0;
        }
        returnBoundary = StartCoroutine(ReturnBoundary());
    }

    //? 회피 - Ground / Water / Food

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
