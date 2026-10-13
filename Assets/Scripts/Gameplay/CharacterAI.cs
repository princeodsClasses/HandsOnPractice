using UnityEngine;

// class07.html 예제 — 6개 상태(StateMachineBehaviour)가 공통으로 참조하는
// AI 설정값 · 런타임 참조 모음이에요. 전투 스탯(hp·공격력 등)은 Character에 따로 있어요.
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CharacterAI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Transform _target;        // 상대(적) Transform
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;

    [Header("감지 · 경계")]
    [SerializeField] private float _detectRange = 8f;
    [SerializeField] private float _suspicionMax = 100f;
    [SerializeField] private float _suspicionRise = 50f;   // 대상이 보일 때, 초당 증가량
    [SerializeField] private float _suspicionFall = 25f;   // 대상이 안 보일 때, 초당 감소량

    [Header("전투")]
    [SerializeField] private float _fireRange = 6f;
    [SerializeField] private float _fireCooldown = 1f;
    [SerializeField, Range(0f, 1f)] private float _fleeHpRatio = 0.3f; // 이 비율 이하면 도주

    [Header("도주")]
    [SerializeField] private float _safeDistance = 10f;    // 이 거리 이상 벌어지면, 경계로 복귀

    [Header("패트롤 · 대기")]
    [SerializeField] private float _arriveThreshold = 0.3f;
    [SerializeField] private Vector2 _idleTimeRange = new(1f, 3f);
    [SerializeField] private float _patrolRadius = 4f; // 시작 위치 기준, 이 범위 안에서 랜덤하게 돌아다녀요.

    public Transform Target => _target;
    public Transform FirePoint => _firePoint;
    public GameObject BulletPrefab => _bulletPrefab;
    public float DetectRange => _detectRange;
    public float SuspicionMax => _suspicionMax;
    public float SuspicionRise => _suspicionRise;
    public float SuspicionFall => _suspicionFall;
    public float FireRange => _fireRange;
    public float FireCooldown => _fireCooldown;
    public float FleeHpRatio => _fleeHpRatio;
    public float SafeDistance => _safeDistance;
    public float ArriveThreshold => _arriveThreshold;
    public Vector2 IdleTimeRange => _idleTimeRange;

    public float Suspicion { get; set; }
    public Vector3 PatrolTarget { get; private set; }

    public Character Stats { get; private set; }
    public Rigidbody Rb { get; private set; }

    private const string GroundName = "Ground";
    private const float GroundMargin = 1f; // 캐릭터 몸집만큼, 가장자리에서 안쪽으로 여유를 둬요.

    private Vector3 _patrolCenter;
    private Vector3 _groundMin;
    private Vector3 _groundMax;
    private bool _hasGroundBounds;
    private bool _isDead;

    void Awake()
    {
        Stats = GetComponent<Character>();
        Rb = GetComponent<Rigidbody>();
        _patrolCenter = transform.position; // 시작 위치를 중심으로, 그 주변을 돌아다녀요.
        PatrolTarget = _patrolCenter;

        GameObject ground = GameObject.Find(GroundName);
        if (ground != null && ground.TryGetComponent<Collider>(out Collider groundCol))
        {
            Bounds b = groundCol.bounds;
            _groundMin = new Vector3(b.min.x + GroundMargin, 0f, b.min.z + GroundMargin);
            _groundMax = new Vector3(b.max.x - GroundMargin, 0f, b.max.z - GroundMargin);
            _hasGroundBounds = true;
        }
    }

    // 모든 상태(Patrol/Chase/Fire/Flee)가 Rigidbody로 움직인 "뒤"에, 바닥 범위 밖으로 안 나가게 눌러줘요.
    // 그리고 HP가 0 이하가 되면, Animator의 "Dead" 트리거를 한 번만 걸어줘요(Any State → Dead).
    void FixedUpdate()
    {
        if (_hasGroundBounds)
        {
            Vector3 pos = Rb.position;
            float clampedX = Mathf.Clamp(pos.x, _groundMin.x, _groundMax.x);
            float clampedZ = Mathf.Clamp(pos.z, _groundMin.z, _groundMax.z);

            if (!Mathf.Approximately(clampedX, pos.x) || !Mathf.Approximately(clampedZ, pos.z))
                Rb.position = new Vector3(clampedX, pos.y, clampedZ);
        }

        if (!_isDead && Stats.Hp <= 0f)
        {
            _isDead = true;
            GetComponent<Animator>().SetTrigger("Dead");
        }
    }

    public float DistanceToTarget() => Vector3.Distance(transform.position, _target.position);

    // 바닥 위 _patrolRadius 범위 안에서, 다음에 갈 랜덤한 지점을 하나 뽑아요(바닥 범위도 같이 고려).
    public void PickNewPatrolPoint()
    {
        Vector2 offset = Random.insideUnitCircle * _patrolRadius;
        Vector3 point = _patrolCenter + new Vector3(offset.x, 0f, offset.y);

        if (_hasGroundBounds)
        {
            point.x = Mathf.Clamp(point.x, _groundMin.x, _groundMax.x);
            point.z = Mathf.Clamp(point.z, _groundMin.z, _groundMax.z);
        }

        PatrolTarget = point;
    }

    // 씬에 배치할 때만 쓰는 설정 — target은 프리팹이 아니라 씬 인스턴스끼리 서로 참조해야 해서,
    // 에디터 스크립트(CombatSceneBuilder)가 배치 시점에 이 메서드로 연결해요.
    public void EditorAssign(Transform target)
    {
        _target = target;
    }

    // Alert · CombatChase · Fire에서 공통으로 쓰는, 대상 쪽으로 수평 회전.
    public void FaceTarget()
    {
        Vector3 dir = _target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
