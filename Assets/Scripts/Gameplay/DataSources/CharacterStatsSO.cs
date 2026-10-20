using UnityEngine;

// class08.html 예제 — 정적 데이터를 ScriptableObject(에셋)로 만들어둔 버전.
// Tiger/Lion 각각 이 타입의 에셋(.asset)을 하나씩 만들어서, Inspector에서 끌어다 끼워요.
[CreateAssetMenu(fileName = "CharacterStats", menuName = "Capstone/Character Stats (ScriptableObject)")]
public class CharacterStatsSO : ScriptableObject
{
    public float hp = 100f;
    public float bulletDamage = 10f;
    public float bulletAngle = 10f;
    public float moveSpeedNormal = 3f;
    public float moveSpeedCombat = 5f;
}
