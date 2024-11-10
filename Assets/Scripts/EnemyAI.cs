using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform player;            // Reference to the player
    public float detectionRange = 10f;   // Range at which zombie detects player
    public float giveUpRange = 15f;      // Range at which zombie gives up the chase
    public float giveUpTime = 3f;        // Time zombie will search before giving up

    private NavMeshAgent agent;          // NavMeshAgent component for movement
    private Animator animator;           // Animator component
    private bool isChasing = false;      // Is zombie chasing the player
    private float searchTimer = 0f;      // Timer for search phase

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // If within detection range, start chasing
            isChasing = true;
            searchTimer = giveUpTime; // Reset search timer
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
        }
        else if (isChasing && distanceToPlayer <= giveUpRange)
        {
            // If out of detection range but still within give up range, keep searching
            searchTimer -= Time.deltaTime;
            if (searchTimer > 0)
            {
                agent.SetDestination(player.position); // Continue to search in last known direction
            }
            else
            {
                // Give up search after timer runs out
                isChasing = false;
                animator.SetBool("isWalking", false);
                agent.SetDestination(transform.position); // Stop moving
            }
        }
        else if (distanceToPlayer > giveUpRange)
        {
            // Give up immediately if the player is outside the give up range
            isChasing = false;
            animator.SetBool("isWalking", false);
            agent.SetDestination(transform.position); // Stop moving
        }
    }

    // Visualize detection and give up ranges in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, giveUpRange);
    }
}