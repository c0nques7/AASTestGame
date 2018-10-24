using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class ZombieHealth : MonoBehaviour
{

    public GameObject[] loot;

    public GameObject FlaotingTextPrefab;
    public GameObject FloatingScorePrefab;
    public GameObject deathAnimation;
    public int startingHealth = 100;            // The amount of health the enemy starts the game with.
    public int currentHealth;                   // The current health the enemy has.
    public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
    public int scoreValue = 10;                      // The amount added to the player's score when the enemy dies.
    public AudioClip deathClip;                 // The sound to play when the enemy dies.
    public AudioClip damagedClip;

    public GameObject thePlayer;

    private int gunDamage;
    


     Animator anim;                              // Reference to the animator.
    AudioSource enemyAudio;                     // Reference to the audio source.
    ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
    CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
    bool isDead;                                // Whether the enemy is dead.
    bool isSinking;                             // Whether the enemy has started sinking through the floor.
    

    private void Start()
    {
        PointCounter.enemies += 1;
    }

    void Awake()
    {
        // Setting up the references.
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        // hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        

        // Setting the current health when the enemy first spawns.
        currentHealth = startingHealth;

        gunDamage = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ArmControllerScript>().ShootSettings.damagePerShot;
        
    }

    void Update()
    {
        

        // If the enemy should be sinking...
        if (isSinking)
        {
            // ... move the enemy down by the sinkSpeed per second.
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }

   

    public void TakeDamage(int amount)
    {
        // If the enemy is dead...
        if (isDead)
            // ... no need to take damage so exit the function.
            return;

        // Play the hurt sound effect.
        enemyAudio.clip = damagedClip;
        enemyAudio.Play();

        // Reduce the current health by the amount of damage sustained.
        currentHealth -= amount;

        // Set the position of the particle system to where the hit was sustained.
        //hitParticles.transform.position = hitPoint;

        // And play the particles.
        //hitParticles.Play();

        //Cause floating text prefab to show up
        if (FlaotingTextPrefab != null)
        {
        ShowFloatingText();

        }

        // If the current health is less than or equal to zero...
        if (currentHealth <= 0)
        {
            // ... the enemy is dead.
            Death();

        }
    }

    void ShowFloatingText()
    {
        

        if (currentHealth >= 0)
        {
            var go = Instantiate(FlaotingTextPrefab, transform.position, Quaternion.identity, transform);
            go.GetComponent<TextMesh>().text = gunDamage.ToString();
        }
    }

    void ShowFloatingScore()
    {
        var go = Instantiate(FloatingScorePrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = "+" + scoreValue.ToString();

    }



    void Death()
    {
        // The enemy is dead.
        isDead = true;

        // Turn the collider into a trigger so shots can pass through it.
        capsuleCollider.isTrigger = true;

        // Increase the score by the enemy's score value.
        PointCounter.points += scoreValue;

        PointCounter.enemies += -1;

        // Tell the animator that the enemy is dead.
        anim.SetTrigger("Dead");

        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
        enemyAudio.clip = deathClip;
        enemyAudio.Play();

        LootDrop();
        StartSinking();

        Instantiate(deathAnimation, gameObject.transform.position, gameObject.transform.rotation);


        Debug.Log("Zombie Killed!");

        if (FloatingScorePrefab != null)
        {
            ShowFloatingScore();

        }
    }

    void LootDrop()
    {
        foreach (GameObject i in loot)
        {
            Instantiate(i, transform.position, transform.rotation);
            Destroy(gameObject, 10f);
            
        }
            
    }


    public void StartSinking()
    {
        // Find and disable the Nav Mesh Agent.
        GetComponent<NavMeshAgent>().enabled = false;

        // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
        GetComponent<Rigidbody>().isKinematic = true;

        // The enemy should now sink.
        isSinking = true;

        

        // After 2 seconds destory the enemy.
        Destroy(gameObject, 2f);
    }

}