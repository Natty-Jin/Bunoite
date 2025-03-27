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
            DontDestroyOnLoad(gameObject);
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
        // 여러 가능한 경로를 시도
        string[] possiblePaths = new string[] 
        {
            "Data/structured_monsters_adjusted_money",
            "Data/structured",
            "Data/monsters",
            "structured_monsters_adjusted_money",
            "structured"
        };

        TextAsset monsterJson = null;
        foreach (string path in possiblePaths)
        {
            Debug.Log($"파일 로드 시도: {path}");
            monsterJson = Resources.Load<TextAsset>(path);
            if (monsterJson != null)
            {
                Debug.Log($"성공! 파일을 찾았습니다: {path}");
                break;
            }
        }

        if (monsterJson != null)
        {
            Debug.Log($"파일 내용: {monsterJson.text}");
            monsterTable = JsonUtility.FromJson<MonsterTable>(monsterJson.text);
            Debug.Log($"몬스터 데이터 로드 완료: {monsterTable.monsters.Length}개의 몬스터");
        }
        else
        {
            Debug.LogError("어떤 경로로도 몬스터 데이터 파일을 찾을 수 없습니다!");
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
            Debug.LogWarning($"해당 구역에 몬스터가 없습니다: {zoneName}");
            return null;
        }
        
        var zoneMonsters = monstersByZone[zoneName]
            .Where(m => Mathf.Abs(m.level - playerLevel) <= 5) // 플레이어 레벨 ±5 범위 내의 몬스터만
            .ToList();
            
        if (zoneMonsters.Count == 0)
        {
            Debug.LogWarning($"해당 레벨에 적합한 몬스터가 없습니다. 레벨: {playerLevel}, 구역: {zoneName}");
            return null;
        }
        
        return zoneMonsters[Random.Range(0, zoneMonsters.Count)];
    }
    
    public List<MonsterDropItem> GetMonsterDrops(Monster monster)
    {
        var drops = new List<MonsterDropItem>();
        
        // 각 아이템의 드롭 확률 계산
        if (Random.value < monster.drop.item1.drop_rate) drops.Add(monster.drop.item1);
        if (Random.value < monster.drop.item2.drop_rate) drops.Add(monster.drop.item2);
        if (Random.value < monster.drop.item3.drop_rate) drops.Add(monster.drop.item3);
        
        return drops;
    }
} 