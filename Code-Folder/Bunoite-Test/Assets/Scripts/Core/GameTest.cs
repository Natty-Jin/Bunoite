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
        // �����̽��ٸ� ������ ���� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.HuntMonster();
        }
    }
}