using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAi : MonoBehaviour
{
    [Header("NavMesh & Target")]
    private NavMeshAgent agent;
    private Transform player;

    [Header("Layers")]
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Enemy Stats")]
    public float health = 100f;
    public float fleeDistance = 100f;
    private bool isDead = false;

    [Header("Patrolling")]
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    [Header("Searching")]
    private Vector3 lastSeenPosition;
    private bool searchingForPlayer;
    public float searchRadius = 5f;

    [Header("Combat")]
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    [Header("UI & Status")]
    public GameObject red, yellow, green;
    public TextMeshProUGUI textHealth;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player")?.transform;
        SetEnemyState(null);
    }

    private void Update()
    {
        if (isDead) return; // Si está muerto, no hace nada

        textHealth.text = health.ToString();
        UpdateState();
    }

    private void UpdateState()
    {
        if (isDead) return; // Evita que siga ejecutando lógica si está muerto

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange)
        {
            lastSeenPosition = player.position;
            searchingForPlayer = false;
        }

        if (health <= 50)
        {
            SetEnemyState(green);
            Flee();
        }
        else if (!playerInSightRange && searchingForPlayer)
        {
            SetEnemyState(yellow);
            SearchForPlayer();
        }
        else if (!playerInSightRange && !playerInAttackRange)
        {
            SetEnemyState(null);
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            SetEnemyState(yellow);
            ChasePlayer();
        }
        else if (playerInAttackRange)
        {
            SetEnemyState(red);
            AttackPlayer();
        }
    }

    private void SetEnemyState(GameObject activeIndicator)
    {
        red.SetActive(activeIndicator == red);
        yellow.SetActive(activeIndicator == yellow);
        green.SetActive(activeIndicator == green);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            if (Vector3.Distance(transform.position, walkPoint) < 1f)
                walkPointSet = false;
        }
    }

    private void SearchForPlayer()
    {
        if (!walkPointSet)
        {
            Vector3 randomPoint = lastSeenPosition + new Vector3(
                Random.Range(-searchRadius, searchRadius),
                0,
                Random.Range(-searchRadius, searchRadius)
            );

            if (Physics.Raycast(randomPoint, Vector3.down, 2f, whatIsGround))
            {
                walkPoint = randomPoint;
                walkPointSet = true;
            }
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            if (Vector3.Distance(transform.position, walkPoint) < 1f)
            {
                walkPointSet = false;
                searchingForPlayer = false;
            }
        }
    }

    private void SearchWalkPoint()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-walkPointRange, walkPointRange),
            0,
            Random.Range(-walkPointRange, walkPointRange)
        );

        walkPoint = transform.position + randomDirection;

        if (Physics.Raycast(walkPoint, Vector3.down, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("ATAQUE");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Flee()
    {
        if (player == null) return;

        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

        if (!Physics.Raycast(fleePosition, Vector3.down, 2f, whatIsGround))
        {
            fleePosition = transform.position + new Vector3(
                Random.Range(-fleeDistance, fleeDistance),
                0,
                Random.Range(-fleeDistance, fleeDistance)
            );
        }

        agent.SetDestination(fleePosition);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Si ya está muerto, no toma más daño

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else if (!playerInSightRange)
        {
            searchingForPlayer = true;
        }
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true; 
        Debug.Log("¡Enemigo ha muerto!");
    }
}
