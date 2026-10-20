using System.Globalization;
using UnityEngine;

// class08.html 예제 — Character의 5개 스탯을, Inspector 직접 입력 대신
// CSV 텍스트(Resources/Data/CharacterStats.csv)에서 이름으로 찾아 읽어와요.
public class CharacterFromCSV : Character
{
    [Header("정적 데이터 소스 (CSV)")]
    [SerializeField] private string _characterKey = "Tiger";
    [SerializeField] private TextAsset _csvFile;

    protected override void LoadExternalStats()
    {
        if (_csvFile == null)
        {
            Debug.LogWarning($"{name}: CSV 파일이 비어있어요. Inspector 값 그대로 써요.");
            return;
        }

        string[] lines = _csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 0번째 줄(헤더)은 건너뛰어요.
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;

            string[] cols = line.Split(',');
            if (cols.Length < 6 || cols[0] != _characterKey) continue;

            SetStats(
                float.Parse(cols[1], CultureInfo.InvariantCulture),
                float.Parse(cols[2], CultureInfo.InvariantCulture),
                float.Parse(cols[3], CultureInfo.InvariantCulture),
                float.Parse(cols[4], CultureInfo.InvariantCulture),
                float.Parse(cols[5], CultureInfo.InvariantCulture)
            );
            return;
        }

        Debug.LogWarning($"{name}: CSV에서 '{_characterKey}' 행을 못 찾았어요.");
    }
}
