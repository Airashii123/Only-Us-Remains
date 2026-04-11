using UnityEngine;
using UnityEngine.AI;

public class Simple3StateEnemy : MonoBehaviour
{
    public HealthBar phealthBar;

    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatisground, whatisplayer;

    [SerializeField] public int ehealth;

    // STATES
    public float sightrange, attackrange;
    public bool playerinsightrange, playerinattackrange;

    // Patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkpointrange;

    // Attack
    public float timebeetweenattacks;
    bool alreadyattacked;

    [Header("Teleport Settings")]
    public float teleportRange = 10f;
    public LayerMask groundLayer;

    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        searchwalkpoint();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        playerinsightrange = Physics.CheckSphere(transform.position, sightrange, whatisplayer);
        playerinattackrange = Physics.CheckSphere(transform.position, attackrange, whatisplayer);

        if (!playerinattackrange && !playerinsightrange)
        {
            Patroling();
        }
        else if (playerinsightrange && !playerinattackrange)
        {
            Chasing();
        }
        else
        {
            Attacking();
        }
    }

    private void Patroling()
    {
        animator.SetBool("IsPatroling", true);
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", false);

        if (!walkPointSet)
        {
            searchwalkpoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
            {
                walkPointSet = false;
            }
        }
    }

    private void searchwalkpoint()
    {
        float randomZ = Random.Range(-walkpointrange, walkpointrange);
        float randomX = Random.Range(-walkpointrange, walkpointrange);

        Vector3 randomPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, walkpointrange, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void Chasing()
    {
        Debug.Log("Chasing");

        animator.SetBool("IsPatroling", false);
        animator.SetBool("IsChasing", true);
        animator.SetBool("IsAttacking", false);

        agent.SetDestination(player.position);
    }

    private void Attacking()
    { 
        if (alreadyattacked) return;

        Debug.Log("Attacking");

        animator.SetBool("IsPatroling", false);
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", true);

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        phealthBar.healthSystem.Damage(10);
        phealthBar.UpdateHealthBar(
            phealthBar.healthSystem.GetHealthPercent()
        );

        alreadyattacked = true;
        Invoke(nameof(resetattack), timebeetweenattacks);
    }

    private void resetattack()
    {
        alreadyattacked = false;
    }

    private void TeleportRandom()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * teleportRange;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, teleportRange, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                return;
            }
        }

        Debug.Log("Nie znaleziono miejsca do teleportu!");
    }
    public void takedamage(int damage)
    {
        ehealth -= damage;

        if (ehealth <= 0)
        {
            Destroy(gameObject);
            return;
        }

        TeleportRandom();
    }

    private void destroyenemy()
    {
        Destroy(gameObject);
    }
}
