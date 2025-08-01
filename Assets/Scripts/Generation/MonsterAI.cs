using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public int damageAmount = 1;
    public float interestPointDuration = 30f;
    public float patrolWaitTime = 3f;

    [Header("References")]
    public LayerMask obstacleLayers;

    [Header ("Links")]
    private PlayerController playerController;
    private Animator animator;
    private AdminPanel adminPanel;

    [Header ("Bools")]
    private bool hasInterestPoint = false;
    private bool isChasing = false;
    private bool isAttacking = false;

    [Header ("Floats")]
    private float lastAttackTime;
    private float interestPointTime;
    private float patrolTimer;

    private Transform player;
    private NavMeshAgent agent;

    private Vector3 lastKnownPlayerPosition;


    private void Start()
    {
        #region Difficulty Settings
        patrolSpeed = DifficultyManager.Instance.CurrentDifficulty.monserRunSpeed;
        chaseSpeed = DifficultyManager.Instance.CurrentDifficulty.monserSprintSpeed;
        detectionRange = DifficultyManager.Instance.CurrentDifficulty.monsterDetectionRange;
        attackRange = DifficultyManager.Instance.CurrentDifficulty.monsterAttackRange;
        #endregion

        playerController = FindAnyObjectByType<PlayerController> ();
        adminPanel = FindAnyObjectByType<AdminPanel>();
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        patrolTimer = patrolWaitTime;

        // Автоматически находим игрока по тегу "Player"
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the tag 'Player'.");
        }
    }

    private void Update()
    {
        UpdateAnimations();

        // Если игрок не найден, пропускаем Update
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            hasInterestPoint = true;
            interestPointTime = Time.time;

            if (distanceToPlayer <= attackRange && !isAttacking && Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
        }
        else if (isChasing)
        {
            if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f)
            {
                StopChasing();
            }
            else
            {
                agent.SetDestination(lastKnownPlayerPosition);
            }
        }
        else
        {
            PatrolOrInvestigate();
        }
    }

    private bool CanSeePlayer()
    {
        if (adminPanel.Invisible == true)
            return false;

        if (Vector3.Distance(transform.position, player.position) > detectionRange)
            return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Проверка угла обзора (например, 120 градусов)
        if (angleToPlayer > 60f)
            return false;

        // Проверка на препятствия
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange, obstacleLayers))
        {
            if (hit.transform != player)
                return false;
        }

        Debug.Log("Can See Player");
        return true;


    }

    private void ChasePlayer()
    {
        Debug.Log("Chase Plauer");

        isChasing = true;
        isAttacking = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void StopChasing()
    {
        Debug.Log("Stop Chasing");

        isChasing = false;
        agent.speed = patrolSpeed;
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack Player");

        isAttacking = true;
        lastAttackTime = Time.time;

        // Наносим урон игроку
        playerController.PlayerHealth -= 1;

        // Отступаем после атаки
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + retreatDirection * 3f);

        Invoke(nameof(ResetAttack), 1f);
    }

    private void ResetAttack()
    {
        Debug.Log("ResetAttack");

        isAttacking = false;
    }

    private void PatrolOrInvestigate()
    {
        // Если есть точка интереса и время еще не истекло
        if (hasInterestPoint && Time.time < interestPointTime + interestPointDuration)
        {
            // С некоторой вероятностью идем к точке интереса
            if (Random.value > 0.7f || Vector3.Distance(transform.position, agent.destination) < 1f)
            {
                agent.SetDestination(lastKnownPlayerPosition + Random.insideUnitSphere * 2f);
                patrolTimer = patrolWaitTime;
            }
            else
            {
                // Обычный патруль
                Patrol();
            }
        }
        else
        {
            hasInterestPoint = false;
            Patrol();
        }
    }

    private void Patrol()
    {
        Debug.Log("Patrol");

        if (!agent.pathPending && (agent.remainingDistance < 1f || patrolTimer <= 0f))
        {
            patrolTimer = patrolWaitTime;
            Vector3 randomPoint = Random.insideUnitSphere * 10f;
            randomPoint += transform.position;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            patrolTimer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация зоны обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Визуализация зоны атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Визуализация последней известной позиции
        if (hasInterestPoint)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(lastKnownPlayerPosition, 0.5f);
        }
    }
    private void UpdateAnimations()
    {
        bool isMoving = agent.velocity.magnitude > 0.1f;
        bool isActuallySprinting = isMoving && isChasing;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", isActuallySprinting);
        animator.SetBool("Attack", false); // Сбрасываем атаку после кадра
    }
}