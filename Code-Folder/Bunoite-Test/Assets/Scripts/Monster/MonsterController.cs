using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterController : MonoBehaviour
{
    [Header("Monster Settings")]
    public Monster monsterData;
    public float maxHealth = 100f;
    public float currentHealth;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    [Header("Combat Settings")]
    public int damage = 10;
    public GameObject hitEffectPrefab;

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime;
    private bool isDead = false;
    private Vector3 knockbackVelocity;
    private float knockbackDuration = 0.5f;
    private float currentKnockbackTime = 0f;
    private bool isKnockedBack = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // 몬스터 데이터 기반으로 스탯 초기화
        if (monsterData != null)
        {
            currentHealth = maxHealth;
            damage = monsterData.level * 5;     // 간단한 데미지 계산
            agent.speed = 3f + monsterData.level * 0.5f; // 레벨에 따른 이동속도
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (isKnockedBack)
        {
            HandleKnockback();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 감지 범위 안에 있을 때
        if (distanceToPlayer <= detectionRange)
        {
            // 공격 범위 안에 있으면 공격
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
                agent.SetDestination(transform.position); // 공격 중에는 제자리에
            }
            // 아니면 플레이어 추적
            else
            {
                agent.SetDestination(player.position);
            }

            // 항상 플레이어를 바라보도록
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        // 감지 범위 밖이면 제자리에
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        // 피격 이펙트 표시
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 데미지 텍스트 표시 (UI 매니저 필요)
        // UIManager.Instance.ShowDamageText(damage, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time;
        
        // 플레이어에게 데미지 주기
        PlayerStats.Instance.TakeDamage(damage);
        
        // 공격 이펙트나 애니메이션 재생
        // TODO: 추가 예정
    }

    private void Die()
    {
        isDead = true;
        agent.enabled = false;

        // 경험치와 돈 지급
        PlayerStats.Instance.GainExp(monsterData.exp);
        PlayerStats.Instance.AddMoney(monsterData.money);

        // 아이템 드롭
        var drops = MonsterManager.Instance.GetMonsterDrops(monsterData);
        foreach (var item in drops)
        {
            Debug.Log($"{item.item} 획득!");
            // TODO: 실제 아이템 생성 또는 인벤토리에 추가
        }

        // 시체 처리
        Destroy(gameObject, 3f); // 3초 후 제거
    }

    public void ApplyKnockback(Vector3 force)
    {
        isKnockedBack = true;
        currentKnockbackTime = knockbackDuration;
        knockbackVelocity = force;
        
        // NavMeshAgent를 일시적으로 비활성화
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = knockbackVelocity;
        }
    }

    private void HandleKnockback()
    {
        if (currentKnockbackTime > 0)
        {
            // 넉백 효과 적용
            transform.position += knockbackVelocity * Time.deltaTime;
            currentKnockbackTime -= Time.deltaTime;
        }
        else
        {
            // 넉백 종료
            isKnockedBack = false;
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
} 