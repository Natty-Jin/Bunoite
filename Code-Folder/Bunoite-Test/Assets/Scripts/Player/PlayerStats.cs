using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;
    public int currentHp = 100;
    public int maxHp = 100;
    public int currentMp = 50;
    public int maxMp = 50;
    public int attackPower = 10;
    public int defense = 5;
    public int money = 0;
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public PlayerData Data { get; private set; }
    
    // 상태 변화를 UI에 알리기 위한 이벤트
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action<int, int> OnManaChanged;
    public event System.Action<int, int> OnExpChanged;
    public event System.Action<int> OnLevelUp;
    public event System.Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePlayerData()
    {
        Data = new PlayerData();
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - Data.defense);
        Data.currentHp = Mathf.Max(0, Data.currentHp - actualDamage);
        OnHealthChanged?.Invoke(Data.currentHp, Data.maxHp);
        
        Debug.Log($"플레이어가 {actualDamage} 데미지를 받았습니다. 남은 체력: {Data.currentHp}/{Data.maxHp}");
        
        if (Data.currentHp <= 0)
        {
            Debug.Log("플레이어가 사망했습니다!");
            // 여기에 사망 처리 로직 추가
        }
    }

    public bool UseMana(int amount)
    {
        if (Data.currentMp >= amount)
        {
            Data.currentMp -= amount;
            OnManaChanged?.Invoke(Data.currentMp, Data.maxMp);
            return true;
        }
        return false;
    }

    public void GainExp(int exp)
    {
        Data.currentExp += exp;
        OnExpChanged?.Invoke(Data.currentExp, Data.maxExp);
        
        Debug.Log($"경험치를 획득했습니다: {exp} (현재: {Data.currentExp}/{Data.maxExp})");
        
        while (Data.currentExp >= Data.maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Data.level++;
        Data.currentExp -= Data.maxExp;
        Data.maxExp = Data.level * 100;  // 간단한 경험치 테이블
        
        // 레벨업 보상
        Data.maxHp += 10;
        Data.maxMp += 5;
        Data.currentHp = Data.maxHp;
        Data.currentMp = Data.maxMp;
        Data.attackPower += 2;
        Data.defense += 1;
        
        Debug.Log($"레벨 업! 현재 레벨: {Data.level}");
        Debug.Log($"HP: {Data.maxHp}, MP: {Data.maxMp}, 공격력: {Data.attackPower}, 방어력: {Data.defense}");
        
        OnLevelUp?.Invoke(Data.level);
        OnHealthChanged?.Invoke(Data.currentHp, Data.maxHp);
        OnManaChanged?.Invoke(Data.currentMp, Data.maxMp);
    }

    public void AddMoney(int amount)
    {
        Data.money += amount;
        Debug.Log($"돈을 획득했습니다: {amount} (현재: {Data.money})");
        OnMoneyChanged?.Invoke(Data.money);
    }
}