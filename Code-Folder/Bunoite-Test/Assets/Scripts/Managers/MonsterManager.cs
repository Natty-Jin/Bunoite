using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }

    public MonsterTable monsterTable;
    private Dictionary<string, List<Monster>> monstersByZone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadMonsterData();
            InitializeMonstersByZone();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadMonsterData()
    {
        try
        {
            string path = "Data/structured_monsters_adjusted_money";
            Debug.Log($"몬스터 데이터 파일 경로: {path}");

            TextAsset monsterJson = Resources.Load<TextAsset>(path);
            Debug.Log($"TextAsset 로드 결과: {(monsterJson != null ? "성공" : "실패")}");

            if (monsterJson != null)
            {
                string jsonContent = monsterJson.text;
                Debug.Log($"JSON 파일 크기: {jsonContent.Length} 바이트");
                Debug.Log($"JSON 내용 미리보기: {(jsonContent.Length > 100 ? jsonContent.Substring(0, 100) + "..." : jsonContent)}");

                monsterTable = JsonUtility.FromJson<MonsterTable>(jsonContent);

                if (monsterTable != null && monsterTable.monsters != null)
                {
                    Debug.Log($"몬스터 데이터 로드 완료: {monsterTable.monsters.Length}개의 몬스터");
                }
                else
                {
                    Debug.LogError("몬스터 테이블 또는 몬스터 배열이 null입니다!");
                }
            }
            else
            {
                // 가능한 경로들을 모두 시도해봅니다
                string[] alternativePaths = {
                    "structured_monsters_adjusted_money",
                    "Data/structured_monsters_adjusted_money.json",
                    "structured_monsters_adjusted_money.json"
                };

                foreach (string altPath in alternativePaths)
                {
                    Debug.Log($"대체 경로 시도: {altPath}");
                    TextAsset altJson = Resources.Load<TextAsset>(altPath);
                    if (altJson != null)
                    {
                        Debug.Log($"대체 경로에서 파일 발견: {altPath}");
                        break;
                    }
                }

                Debug.LogError($"몬스터 데이터 파일을 찾을 수 없습니다! 원본 경로: {path}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"몬스터 데이터 로드 중 오류 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    private void InitializeMonstersByZone()
    {
        monstersByZone = new Dictionary<string, List<Monster>>();

        foreach (var monster in monsterTable.monsters)
        {
            if (!monstersByZone.ContainsKey(monster.location))
            {
                monstersByZone[monster.location] = new List<Monster>();
            }
            monstersByZone[monster.location].Add(monster);
        }
    }

    public Monster GetRandomMonsterForZone(string zoneName, int playerLevel)
    {
        if (!monstersByZone.ContainsKey(zoneName))
        {
            Debug.LogWarning($"존 데이터에서 몬스터를 찾을 수 없습니다: {zoneName}");
            return null;
        }

        var zoneMonsters = monstersByZone[zoneName]
            .Where(m => Mathf.Abs(m.level - playerLevel) <= 5) // 플레이어 레벨 기준 5레벨 이내 몬스터만 추출
            .ToList();

        if (zoneMonsters.Count == 0)
        {
            Debug.LogWarning($"존 데이터에서 몬스터를 찾을 수 없습니다. 플레이어 레벨: {playerLevel}, 존: {zoneName}");
            return null;
        }

        return zoneMonsters[Random.Range(0, zoneMonsters.Count)];
    }

    public List<MonsterDropItem> GetMonsterDrops(Monster monster)
    {
        var drops = new List<MonsterDropItem>();

        // 확률을 이용한 드랍 아이템 추출
        if (Random.value < monster.drop.item1.drop_rate) drops.Add(monster.drop.item1);
        if (Random.value < monster.drop.item2.drop_rate) drops.Add(monster.drop.item2);
        if (Random.value < monster.drop.item3.drop_rate) drops.Add(monster.drop.item3);

        return drops;
    }
}