using TMPro;
using UnityEngine;
using UnityEngine.UI;

// class07.html 예제 — 사자/호랑이가 공통으로 쓰는 스탯 데이터예요.
// 클래스 하나를 공유하고, 프리팹(Tiger / Lion)마다 Inspector 값만 다르게 넣어요.
public class Character : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float _hp = 100f;

    [Header("공격")]
    [SerializeField] private float _bulletDamage = 10f;   // 발사하는 총알의 공격력
    [SerializeField] private float _bulletAngle = 10f;    // 총알 공격 각도 — 조준 방향에서 퍼지는 범위(도)

    [Header("이동 속도")]
    [SerializeField] private float _moveSpeedNormal = 3f;  // 평소(비전투) 이동 속도
    [SerializeField] private float _moveSpeedCombat = 5f;  // 전투 중 이동 속도

    [Header("머리 위 표시")]
    [SerializeField] private Image _hpFill;        // HP 게이지 (Image.Type.Filled)
    [SerializeField] private TMP_Text _stateLabel;  // 게이지 위, 현재 상태 이름

    private float _maxHp;

    public float Hp => _hp;
    public float MaxHp => _maxHp;
    public float BulletDamage => _bulletDamage;
    public float BulletAngle => _bulletAngle;
    public float MoveSpeedNormal => _moveSpeedNormal;
    public float MoveSpeedCombat => _moveSpeedCombat;

    void Awake()
    {
        _maxHp = _hp; // Inspector에 넣은 값을, 최대치로 기억해둬요 — 전투 도주 조건(체력 비율) 판정에 써요.
        UpdateHpBar();
    }

    public void TakeDamage(float amount)
    {
        _hp -= amount;
        UpdateHpBar();
    }

    public void SetStateLabel(string label)
    {
        if (_stateLabel != null)
            _stateLabel.text = label;
    }

    private void UpdateHpBar()
    {
        if (_hpFill != null)
            _hpFill.fillAmount = _maxHp > 0f ? Mathf.Clamp01(_hp / _maxHp) : 0f;
    }
}
