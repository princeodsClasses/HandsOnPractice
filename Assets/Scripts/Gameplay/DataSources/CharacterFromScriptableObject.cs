using UnityEngine;

// class08.html 예제 — Character의 5개 스탯을, Inspector 직접 입력 대신
// ScriptableObject 에셋(CharacterStatsSO)에서 읽어와요.
public class CharacterFromScriptableObject : Character
{
    [Header("정적 데이터 소스 (ScriptableObject)")]
    [SerializeField] private CharacterStatsSO _statsAsset;

    protected override void LoadExternalStats()
    {
        if (_statsAsset == null)
        {
            Debug.LogWarning($"{name}: CharacterStatsSO가 비어있어요. Inspector 값 그대로 써요.");
            return;
        }

        SetStats(
            _statsAsset.hp,
            _statsAsset.bulletDamage,
            _statsAsset.bulletAngle,
            _statsAsset.moveSpeedNormal,
            _statsAsset.moveSpeedCombat
        );
    }
}
