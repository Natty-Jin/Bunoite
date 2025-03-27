using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int requiredExp;
    public int gold = 0;

    private MonsterZone currentZone;

    private void Start()
    {
        UpdateRequiredExp();
        currentZone = GameManager.Instance.GetZoneForLevel(level);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득: {amount} (현재: {currentExp}/{requiredExp})");

        while (currentExp >= requiredExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= requiredExp;
        level++;
        UpdateRequiredExp();
        Debug.Log($"레벨 업! 현재 레벨: {level}");

        MonsterZone newZone = GameManager.Instance.GetZoneForLevel(level);
        if (newZone != null && newZone != currentZone)
        {
            currentZone = newZone;
            Debug.Log($"새로운 구역 개방: {currentZone.zone} - {currentZone.theme}");
        }
    }

    private void UpdateRequiredExp()
    {
        requiredExp = GameManager.Instance.GetRequiredExpForLevel(level);
    }

    public void HuntMonster()
    {
        if (currentZone == null)
        {
            Debug.LogWarning("현재 사냥 가능한 구역이 없습니다.");
            return;
        }

        Monster monster = MonsterManager.Instance.GetRandomMonsterForZone(currentZone.zone, level);
        if (monster == null)
        {
            Debug.LogWarning("이 구역에서 사냥할 수 있는 몬스터가 없습니다.");
            return;
        }

        Debug.Log($"{monster.name} 발견! (Lv.{monster.level})");

        // 몬스터 처치 (실제 게임에서는 전투 시스템으로 구현)
        GainExp(monster.exp);
        gold += monster.money;
        Debug.Log($"골드 획득: {monster.money} (현재: {gold})");

        // 아이템 드롭
        List<MonsterDropItem> drops = MonsterManager.Instance.GetMonsterDrops(monster);
        foreach (var drop in drops)
        {
            Debug.Log($"아이템 획득: {drop.item} ({drop.type}) - {drop.use}");
        }
    }
}