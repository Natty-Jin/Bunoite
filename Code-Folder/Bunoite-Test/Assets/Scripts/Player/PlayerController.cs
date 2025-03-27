using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;

    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float knockbackForce = 5f;
    public LayerMask monsterLayer;
    public LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 velocity;
    private float lastAttackTime;
    private Transform cameraTransform;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        lastAttackTime = -attackCooldown;
        
        // 메인 카메라 찾기
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // 마우스 커서 잠그기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleAttack();
        
        // ESC로 커서 잠금 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CheckGrounded()
    {
        // 지면 체크를 위한 레이캐스트
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
        
        // 지면에 닿았을 때 속도 초기화
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        // 이동 입력
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 카메라 기준으로 이동 방향 계산
        Vector3 move = Vector3.zero;
        if (cameraTransform != null)
        {
            // 카메라의 전방 벡터에서 y값을 0으로 만들어 수평 이동만 처리
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            move = forward * vertical + right * horizontal;
        }
        else
        {
            // 카메라가 없는 경우 월드 좌표 기준으로 이동
            move = transform.right * horizontal + transform.forward * vertical;
        }

        // 이동 적용
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 점프 처리
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            Debug.Log("Jump!");
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 이동 방향으로 회전
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log("Attack!");

        // 전방의 몬스터 감지
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position + transform.forward * attackRange/2, 
            attackRange/2, 
            monsterLayer
        );

        foreach (var hitCollider in hitColliders)
        {
            var monsterController = hitCollider.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                Debug.Log($"Hit monster: {hitCollider.name}");
                
                // 몬스터에게 데미지를 주고 넉백 적용
                monsterController.TakeDamage(PlayerStats.Instance.Data.attackPower);
                
                // 넉백 방향 계산 (플레이어로부터 몬스터 방향)
                Vector3 knockbackDirection = (hitCollider.transform.position - transform.position).normalized;
                knockbackDirection.y = 0; // Y축 넉백 제거
                
                // 몬스터에게 넉백 적용
                monsterController.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange/2, attackRange/2);
    }
} 