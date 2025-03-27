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
        Debug.Log($"����ġ ȹ��: {amount} (����: {currentExp}/{requiredExp})");

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
        Debug.Log($"���� ��! ���� ����: {level}");

        MonsterZone newZone = GameManager.Instance.GetZoneForLevel(level);
        if (newZone != null && newZone != currentZone)
        {
            currentZone = newZone;
            Debug.Log($"���ο� ���� ����: {currentZone.zone} - {currentZone.theme}");
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
            Debug.LogWarning("���� ��� ������ ������ �����ϴ�.");
            return;
        }

        Monster monster = MonsterManager.Instance.GetRandomMonsterForZone(currentZone.zone, level);
        if (monster == null)
        {
            Debug.LogWarning("�� �������� ����� �� �ִ� ���Ͱ� �����ϴ�.");
            return;
        }

        Debug.Log($"{monster.name} �߰�! (Lv.{monster.level})");

        // ���� óġ (���� ���ӿ����� ���� �ý������� ����)
        GainExp(monster.exp);
        gold += monster.money;
        Debug.Log($"��� ȹ��: {monster.money} (����: {gold})");

        // ������ ���
        List<MonsterDropItem> drops = MonsterManager.Instance.GetMonsterDrops(monster);
        foreach (var drop in drops)
        {
            Debug.Log($"������ ȹ��: {drop.item} ({drop.type}) - {drop.use}");
        }
    }
}