using UnityEngine;

public class TestMonsterSpawn : MonoBehaviour
{
    public int playerLevel = 1;  // Inspector에서 수정 가능
    public string zoneName = "사막 분지";  // Inspector에서 수정 가능

    void Update()
    {
        // 스페이스바를 누르면 몬스터 생성 테스트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMonster();
        }
    }

    void SpawnMonster()
    {
        if (MonsterManager.Instance == null)
        {
            Debug.LogError("MonsterManager가 없습니다!");
            return;
        }

        Monster monster = MonsterManager.Instance.GetRandomMonsterForZone(zoneName, playerLevel);
        
        if (monster != null)
        {
            Debug.Log($"생성된 몬스터: {monster.name} (레벨: {monster.level})");
            Debug.Log($"속성: {monster.element}, 위치: {monster.location}");
            
            var drops = MonsterManager.Instance.GetMonsterDrops(monster);
            if (drops.Count > 0)
            {
                Debug.Log("드롭 아이템:");
                foreach (var item in drops)
                {
                    Debug.Log($"- {item.item} ({item.type}): {item.use}");
                }
            }
            else
            {
                Debug.Log("드롭된 아이템이 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"{zoneName} 지역에서 레벨 {playerLevel} 플레이어에 맞는 몬스터를 찾을 수 없습니다.");
        }
    }
} 