using UnityEngine;

public class GameTest : MonoBehaviour
{
    private PlayerCharacter player;
    
    void Start()
    {
        player = GetComponent<PlayerCharacter>();
    }
    
    void Update()
    {
        // 스페이스바를 누르면 몬스터 사냥
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.HuntMonster();
        }
    }
} 