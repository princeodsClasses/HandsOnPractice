using System.Linq;
using System.Xml.Linq;
using UnityEngine;

// class08.html 예제 — Character의 5개 스탯을, Inspector 직접 입력 대신
// XML 텍스트(Resources/Data/CharacterStats.xml)에서 이름으로 찾아 읽어와요.
public class CharacterFromXML : Character
{
    [Header("정적 데이터 소스 (XML)")]
    [SerializeField] private string _characterKey = "Tiger";
    [SerializeField] private TextAsset _xmlFile;

    protected override void LoadExternalStats()
    {
        if (_xmlFile == null)
        {
            Debug.LogWarning($"{name}: XML 파일이 비어있어요. Inspector 값 그대로 써요.");
            return;
        }

        XDocument doc = XDocument.Parse(_xmlFile.text);
        XElement node = doc.Root?.Elements("CharacterStats")
            .FirstOrDefault(e => (string)e.Attribute("name") == _characterKey);

        if (node == null)
        {
            Debug.LogWarning($"{name}: XML에서 '{_characterKey}' 노드를 못 찾았어요.");
            return;
        }

        SetStats(
            (float)node.Element("hp"),
            (float)node.Element("bulletDamage"),
            (float)node.Element("bulletAngle"),
            (float)node.Element("moveSpeedNormal"),
            (float)node.Element("moveSpeedCombat")
        );
    }
}
