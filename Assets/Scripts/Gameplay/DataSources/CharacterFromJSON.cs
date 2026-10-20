using UnityEngine;

// class08.html 예제 — Character의 5개 스탯을, Inspector 직접 입력 대신
// JSON 텍스트(Resources/Data/CharacterStats.json)에서 이름으로 찾아 읽어와요.
public class CharacterFromJSON : Character
{
    [Header("정적 데이터 소스 (JSON)")]
    [SerializeField] private string _characterKey = "Tiger";
    [SerializeField] private TextAsset _jsonFile;

    protected override void LoadExternalStats()
    {
        if (_jsonFile == null)
        {
            Debug.LogWarning($"{name}: JSON 파일이 비어있어요. Inspector 값 그대로 써요.");
            return;
        }

        // JsonUtility는 최상위가 배열인 JSON은 못 읽어요 — { "list": [...] } 처럼
        // 한 번 감싸는 래퍼(CharacterStatsJsonList)가 필요해요.
        CharacterStatsJsonList data = JsonUtility.FromJson<CharacterStatsJsonList>(_jsonFile.text);
        if (data?.list == null)
        {
            Debug.LogWarning($"{name}: JSON 파싱에 실패했어요.");
            return;
        }

        foreach (CharacterStatsJsonEntry entry in data.list)
        {
            if (entry.name != _characterKey) continue;

            SetStats(entry.hp, entry.bulletDamage, entry.bulletAngle, entry.moveSpeedNormal, entry.moveSpeedCombat);
            return;
        }

        Debug.LogWarning($"{name}: JSON에서 '{_characterKey}' 항목을 못 찾았어요.");
    }
}

[System.Serializable]
public class CharacterStatsJsonEntry
{
    public string name;
    public float hp;
    public float bulletDamage;
    public float bulletAngle;
    public float moveSpeedNormal;
    public float moveSpeedCombat;
}

[System.Serializable]
public class CharacterStatsJsonList
{
    public CharacterStatsJsonEntry[] list;
}
