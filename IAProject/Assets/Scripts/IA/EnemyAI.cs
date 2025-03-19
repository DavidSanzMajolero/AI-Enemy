using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.UI;
using TMPro;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100; 

    public float fleeDistance = 100f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public GameObject red;
    public GameObject yellow;
    public GameObject green;

    public TextMeshProUGUI textHealth;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        red.SetActive(false);
        yellow.SetActive(false);
        green.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        //raycast para el player

        //guardar la ultima posicion del player y hacer que el enemigo patruye en esa posicion

        //cambiar el estado de patrullaje por un collider, y llamarlo primero en el start y en el triggerstay cambiar a chaseplayer o a attack dependiendo de la distancia del player (si el raycast detecta al player lo chasea, sino sigue en patrol)

    }
    private void Update()
    {
        textHealth.text = health.ToString();
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (health <= 50)
        {
            Debug.Log("Estado: Huir");
            red.SetActive(false);
            yellow.SetActive(false);
            green.SetActive(true);
            Flee();
        }
        else if (!playerInSightRange && !playerInAttackRange)
        {
            Debug.Log("Estado: Patrullando");
            red.SetActive(false);
            yellow.SetActive(false);
            green.SetActive(false);
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Debug.Log("Estado: Persiguiendo al jugador");
            red.SetActive(false);
            yellow.SetActive(true);
            green.SetActive(false);
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            Debug.Log("Estado: Atacando al jugador");
            red.SetActive(true);
            yellow.SetActive(false);
            green.SetActive(false);
            AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("ATAQUEE");
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
        Vector3 directionToFlee = transform.position - player.position;
 
        Vector3 fleePosition = transform.position + directionToFlee.normalized * fleeDistance;
        if (Physics.Raycast(fleePosition, -transform.up, 2f, whatIsGround))
        {
            agent.SetDestination(fleePosition);
        }
        else
        {
            fleePosition = transform.position + new Vector3(Random.Range(-fleeDistance, fleeDistance), 0, Random.Range(-fleeDistance, fleeDistance));
            agent.SetDestination(fleePosition);
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

}
