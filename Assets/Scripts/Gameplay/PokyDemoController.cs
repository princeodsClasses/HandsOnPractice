using System.Collections;
using UnityEngine;

// class07.html 레이어/블렌드트리 예제 — Player_Poki.controller를 직접 조작해요.
// 기본: 대기. W: 걷기. Shift+W: 달리기. 1~5: 무기 교체(기본 Pistol). 좌클릭 유지: 락온(Fire Layer).
[RequireComponent(typeof(Animator))]
public class PokyDemoController : MonoBehaviour
{
    private const int FireLayerIndex = 1; // Base Layer(0) / Fire Layer(1, AvatarMask) / Fire Move Layer(2, 미사용)

    // Player_Poki.controller의 WeaponAnimType 값 — Pistol/Rifle/Shotgun/Sniper/Minigun 순서(숫자키 1~5와 매칭).
    [SerializeField] private int[] _weaponAnimTypes = { 1, 3, 14, 7, 9 };

    // 애니메이션(WeaponAnimType)과 같은 순서 — 실제로 손에 들리는 무기 프리팹(Assets/Prefabs/Weapons).
    // Inspector에서 직접 채워주세요 — 무기 구성이 바뀔 걸 생각하면, 코드에 경로를 박아두는 것보단 나아요.
    [SerializeField] private GameObject[] _weaponPrefabs;

    private const string HandBoneName = "Dummy_Weapon"; // 그립 정렬이 이미 세팅된 무기 전용 소켓(오른손 본의 자식).
    private const string SpineBoneName = "Bip001 Spine1";
    private const float LookBlendSpeed = 6f;   // 락온 켜짐/꺼짐 시, 룩앳 가중치가 부드럽게 늘어나는 속도.
    private const float MaxLookYaw = 60f;      // 캐릭터 정면 기준, 좌우로 더 돌아갈 수 있는 최대 각도.
    private const float MaxLookPitch = 40f;    // 위/아래 최대 각도.
    private const float LookTurnSpeed = 360f;  // 목표 각도를 초당 이 속도로만 따라가요(순간 이동 금지 — 뒤쪽 180도 경계에서 튀는 것 방지).
    private const float AimRayDistance = 20f;  // 카메라 레이 위에서, 이 거리만큼 떨어진 지점을 조준점으로 써요.
    private const float ScreenDeadzoneRadius = 0.12f;  // 뷰포트(0~1) 기준, 화면 중앙에서 이 반경 안이면 룩앳 안 함.
    private const float ScreenDeadzoneFeather = 0.1f;  // 데드존 경계 바깥, 이 폭만큼 부드럽게 램프업.
    private const float FireLayerBlendSpeed = 8f;      // Fire Layer weight가 0↔1로 초당 이 속도로 블렌딩돼요(즉시 토글 아님).

    private Animator _anim;
    private Transform _spineBone;
    private Transform _handBone;
    private Camera _cam;
    private GameObject _equippedWeapon;
    private int _weaponSlot;
    private bool _isLockedOn;
    private bool _isReloading;
    private float _fireLayerWeight;
    private float _lookWeight;
    private float _yaw;
    private float _pitch;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _spineBone = FindDeepChild(transform, SpineBoneName);
        _handBone = FindDeepChild(transform, HandBoneName);
        _cam = Camera.main;

        // BTree 상태들의 재생 속도 파라미터 — 0이면 애니메이션이 멈춰(freeze) 보여요.
        _anim.SetFloat("AnimSpeed", 1f);
        _anim.SetFloat("MoveSpeedRatio", 1f);
        _anim.SetFloat("ReloadSpeed", 1f);

        EquipWeapon(_weaponSlot);
        _anim.SetLayerWeight(FireLayerIndex, 0f);
    }

    // class06.html PART3의 ResourceManager 풀을 그대로 써요 — 매번 Instantiate/Destroy 대신,
    // 꺼낸 무기는 Return으로 풀에 돌려놔요. 무기 프리팹의 WeaponModelInfo._tHandDummy는
    // 그 프리팹 "자기 자신의 루트"를 가리켜서, 손 본에 붙이고 로컬 좌표를 0으로 두면
    // 미리 세팅된 그립 위치에 바로 맞아들어가요.
    void EquipWeapon(int slot)
    {
        if (_weaponPrefabs == null || slot >= _weaponPrefabs.Length)
        {
            Debug.LogWarning($"{name}: _weaponPrefabs가 비어있어요 — Inspector에서 무기 프리팹 {_weaponAnimTypes.Length}개를 채워주세요.");
            return;
        }

        if (_equippedWeapon != null)
            ResourceManager.Singleton.Return(_weaponPrefabs[_weaponSlot].name, _equippedWeapon);

        _weaponSlot = slot;
        if (_handBone == null || _weaponPrefabs[slot] == null) return;

        GameObject prefab = _weaponPrefabs[slot];
        GameObject instance = ResourceManager.Singleton.Get(prefab.name, prefab);

        Transform dummy = instance.GetComponent<WeaponModelInfo>()?.GetHandDummyTransform() ?? instance.transform;
        dummy.SetParent(_handBone, false);
        dummy.localPosition = Vector3.zero;
        dummy.localRotation = Quaternion.identity;

        _equippedWeapon = instance;
        _anim.SetFloat("WeaponAnimType", _weaponAnimTypes[slot]);
    }

    static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }

    void Update()
    {
        HandleWeaponSwitch();
        HandleMove();
        HandleLockOn();
        HandleReload();
        UpdateFireLayerWeight();
    }

    // 락온 중이거나 리로드 중이면 목표 weight는 1, 아니면 0 — 그 값을 바로 때려넣지 않고
    // 매 프레임 조금씩 따라가게 해서, 0/1 즉시 토글 대신 짧게라도 블렌딩되게 해요.
    void UpdateFireLayerWeight()
    {
        float target = (_isLockedOn || _isReloading) ? 1f : 0f;
        _fireLayerWeight = Mathf.MoveTowards(_fireLayerWeight, target, FireLayerBlendSpeed * Time.deltaTime);
        _anim.SetLayerWeight(FireLayerIndex, _fireLayerWeight);
    }

    void HandleWeaponSwitch()
    {
        for (int i = 0; i < _weaponAnimTypes.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i != _weaponSlot)
            {
                EquipWeapon(i);
                return;
            }
        }
    }

    // 포키는 제자리에서 애니메이션만 재생해요 — 실제 이동은 안 시켜요.
    void HandleMove()
    {
        bool isMove = Input.GetKey(KeyCode.W);
        bool isRun = isMove && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        _anim.SetBool("IsMove", isMove);
        _anim.SetFloat("Speed", isRun ? 1f : 0f);
    }

    // 좌클릭을 누르고 있는 동안만 Fire Layer를 켜요 — 마스크(상체)로만 조준 포즈가 덧씌워지고,
    // 다리는 Base Layer가 그대로 걷기/달리기를 재생해요. Fire Move Layer는 안 써요(마스크 블렌딩만으로 충분).
    void HandleLockOn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isLockedOn = true;
            _anim.SetTrigger("LockOn");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isLockedOn = false;
        }
    }

    // 락온 여부와 상관없이 R이면 리로드해요. Reload_BTree가 Fire Layer(마스크) 안에 있어서
    // 레이어 weight가 0이면 재생해도 안 보이니까, 락온 중이 아니면 리로드 끝날 때까지만
    // 레이어를 잠깐 켰다가, 끝나면(그리고 그사이 락온도 안 했으면) 다시 꺼요.
    void HandleReload()
    {
        if (!_isReloading && Input.GetKeyDown(KeyCode.R))
            StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        _anim.SetTrigger("Reload");

        // 트랜지션(크로스페이드 0.15초)이 끝나고 실제로 Reload_BTree에 "들어갈 때"까지 먼저 기다려요 —
        // 한 프레임만 기다리고 바로 체크하면, 아직 이전 상태 크로스페이드 중이라 Reload_BTree가 아니라고
        // 오판해서 바로 꺼버려요(눌러도 움찔하고 끝나는 증상의 원인).
        float timeout = 1f;
        while (!_anim.GetCurrentAnimatorStateInfo(FireLayerIndex).IsName("Reload_BTree") && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        // 그다음, Reload_BTree를 "벗어날 때"까지 기다려요.
        while (_anim.GetCurrentAnimatorStateInfo(FireLayerIndex).IsName("Reload_BTree"))
            yield return null;

        _isReloading = false;
    }

    // Walk/Run 등 이동 클립에 박혀있는 AnimationEvent("Footstep")가 호출할 리시버.
    // 발소리 SFX는 아직 안 붙여서 받기만 하고 그냥 리턴.
    void Footstep()
    {
        Debug.Log("탁");
    }

    // 락온 중엔 상체(Spine1)가 마우스 포인터 방향을 향하도록, Animator가 포즈를 다 그린 "뒤"(LateUpdate)에
    // 추가로 회전시켜요. Poky는 Humanoid가 아니라 Generic 리그라 SetLookAtPosition 같은 기본 IK를 못 써요.
    // 그래서 본의 로컬 축 방향과 상관없이 동작하도록, "애니메이션 결과 위에 월드 기준 yaw/pitch를 얹는" 방식으로 처리해요.
    void LateUpdate()
    {
        if (_spineBone == null || _cam == null) return;

        _lookWeight = Mathf.MoveTowards(_lookWeight, _isLockedOn ? 1f : 0f, LookBlendSpeed * Time.deltaTime);
        if (_lookWeight <= 0f) return;

        // 화면 중앙에서 마우스까지의 "뷰포트 거리"로 데드존을 둬요 — 캐릭터 정면 기준 각도로 재면,
        // 카메라가 캐릭터 정면과 다른 방향을 보고 있을 때 "화면 중앙"이랑 안 맞아떨어져서 이상하게 느껴져요.
        Vector3 viewportPos = _cam.ScreenToViewportPoint(Input.mousePosition);
        float centerDist = Vector2.Distance(viewportPos, new Vector2(0.5f, 0.5f));
        float activeAmount = Mathf.SmoothStep(0f, 1f,
            Mathf.InverseLerp(ScreenDeadzoneRadius, ScreenDeadzoneRadius + ScreenDeadzoneFeather, centerDist));

        // 카메라 레이 위의 고정 거리 지점을 조준점으로 써요 — 평면 교차 방식은 레이가 평면과 거의
        // 평행해지는 화면상 "수평선" 근처(마우스를 위로 올릴 때)에서 교차점이 무한히 멀어지며 값이
        // 널뛰는데, 고정 거리 지점은 화면 어디를 봐도 안정적이에요.
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Vector3 aimPoint = ray.GetPoint(AimRayDistance);
        Vector3 toAim = aimPoint - _spineBone.position;

        Vector3 flatForward = transform.forward;
        Vector3 flatAim = new Vector3(toAim.x, 0f, toAim.z);
        if (flatAim.sqrMagnitude < 0.0001f) return;
        flatAim.Normalize();

        float rawYaw = Mathf.Clamp(Vector3.SignedAngle(flatForward, flatAim, Vector3.up), -MaxLookYaw, MaxLookYaw);
        // transform.right를 축으로 한 부호가 "위를 보면 양수 피치"와 반대로 나와서, 마지막에 뒤집어줘요.
        float rawPitch = -Mathf.Clamp(Vector3.SignedAngle(flatAim, toAim.normalized, transform.right), -MaxLookPitch, MaxLookPitch);

        // 데드존 안쪽(activeAmount=0)이면 정면(0도)으로, 바깥쪽(activeAmount=1)이면 원래 각도 그대로.
        float targetYaw = rawYaw * activeAmount;
        float targetPitch = rawPitch * activeAmount;

        // 목표 각도로 즉시 스냅하지 않고 속도 제한을 둬요 — 조준 방향이 캐릭터 뒤쪽(±180°) 경계를 넘을 때
        // SignedAngle 값이 +180/-180으로 순간 뒤집히는데, 그걸 그대로 적용하면 +60°↔-60°로 튀어 보여요.
        _yaw = Mathf.MoveTowardsAngle(_yaw, targetYaw, LookTurnSpeed * Time.deltaTime);
        _pitch = Mathf.MoveTowardsAngle(_pitch, targetPitch, LookTurnSpeed * Time.deltaTime);

        Quaternion extraRotation = Quaternion.AngleAxis(_yaw, Vector3.up) * Quaternion.AngleAxis(_pitch, transform.right);
        _spineBone.rotation = Quaternion.Slerp(Quaternion.identity, extraRotation, _lookWeight) * _spineBone.rotation;
    }
}
