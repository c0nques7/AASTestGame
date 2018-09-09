using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    Transform player;               // Reference to the player's position.
    PlayerHealth playerHealth;      // Reference to the player's health.
    ZombieHealth zombieHealth;        // Reference to this enemy's health.
    NavMeshAgent nav;               // Reference to the nav mesh agent.
    Animator anim;                  // Reference to the animator


    void Awake()
    {
        // Set up the references.
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        zombieHealth = GetComponent<ZombieHealth>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        // If the enemy and the player have health left...
        if (zombieHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        {
            // ... set the destination of the nav mesh agent to the player.
            nav.SetDestination(player.position);
            anim.SetTrigger("Walk");
        }
        // Otherwise...
        else
        {
            // ... disable the nav mesh agent.
            nav.enabled = false;
        }
    }
}

