using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float fleeSpeed = 7f;
    [Tooltip("Дистанция, после которой монстр теряет игрока")]
    public float loseDistance = 20f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 2f;
    [Tooltip("Дистанция побега после атаки")]
    public float fleeDistance = 10f;

    [Header("Animations")]
    private Animator animator;
    public string runParam = "Run";
    public string attackTrigger = "Attack";
    public string fleeParam = "Flee";

    private Transform player;
    private NavMeshAgent agent;
    private bool isFleeing;
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        agent.speed = walkSpeed;
    }

    void Update()
    {
        if (isFleeing) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Режим преследования
        if (distance < loseDistance && CanSeePlayer())
        {
            ChasePlayer();
            if (distance <= agent.stoppingDistance && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            Patrol();
        }
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out hit, loseDistance))
        {
            return hit.transform == player;
        }
        return false;
    }

    private void ChasePlayer()
    {
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        animator.SetBool(runParam, true);
    }

    private void Patrol()
    {
        if (!agent.isOnNavMesh)
        {
            // Пытаемся перенести агента на NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // Телепортация на валидную позицию
            }
            return;
        }

        if (agent.remainingDistance < 1f)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
    private void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger(attackTrigger);
        // Нанесение урона (раскомментируйте)
        // player.GetComponent<PlayerHealth>().TakeDamage(damage);
        StartCoroutine(Flee());
    }

    private System.Collections.IEnumerator Flee()
    {
        isFleeing = true;
        animator.SetBool(fleeParam, true);

        Vector3 fleeDirection = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + fleeDirection * fleeDistance);
        agent.speed = fleeSpeed;

        yield return new WaitForSeconds(3f); // Длительность бегства

        isFleeing = false;
        animator.SetBool(fleeParam, false);
    }
}