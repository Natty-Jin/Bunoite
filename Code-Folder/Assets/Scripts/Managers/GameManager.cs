using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public ExpTable expTable;
    public MonsterZoneTable zoneTable;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadGameData()
    {
        TextAsset expJson = Resources.Load<TextAsset>("Data/exp_table");
        TextAsset zoneJson = Resources.Load<TextAsset>("Data/monster_zones");
        
        if (expJson != null)
            expTable = JsonUtility.FromJson<ExpTable>(expJson.text);
            
        if (zoneJson != null)
            zoneTable = JsonUtility.FromJson<MonsterZoneTable>(zoneJson.text);
    }

    public MonsterZone GetZoneForLevel(int level)
    {
        foreach (var zone in zoneTable.zones)
        {
            string[] range = zone.range.Split('-');
            int minLevel = int.Parse(range[0]);
            int maxLevel = int.Parse(range[1]);
            
            if (level >= minLevel && level <= maxLevel)
                return zone;
        }
        return null;
    }

    public int GetRequiredExpForLevel(int level)
    {
        foreach (var expData in expTable.expTable)
        {
            if (expData.level == level)
                return expData.required_exp;
        }
        return 0;
    }
} 