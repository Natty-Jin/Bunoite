using System;

[Serializable]
public class MonsterDropItem
{
    public string item;
    public string type;
    public string use;
    public float drop_rate;
}

[Serializable]
public class MonsterDrop
{
    public MonsterDropItem item1;
    public MonsterDropItem item2;
    public MonsterDropItem item3;
}

[Serializable]
public class Monster
{
    public int level;
    public string name;
    public string location;
    public string condition;
    public bool boss;
    public string element;
    public MonsterDrop drop;
    public int money;
    public int exp;
}

[Serializable]
public class MonsterTable
{
    public string description;
    public Monster[] monsters;
}