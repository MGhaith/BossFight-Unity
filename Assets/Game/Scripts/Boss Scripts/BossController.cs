using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    //variables
    public enum BossAttackPhase
    {
        Phase1,
        Phase2,
        Phase3,
        Phase4
    }

    private Rigidbody2D rb;
    public GameObject player;
    public bool isFlipped = false;
    public BossAttackPhase APhase;
    public float movementSpeed = 5f; // Boss movement speed
    public GameObject projectilePrefab;

    private bool isIdentiSpawned = false;
    private float timer = 0f;
    public float timeInterval = 10f; // Interval to call the method
    private SpriteRenderer bossRenderer;


    // Start is called before the first frame update
    void Start()
    {
        APhase = BossAttackPhase.Phase1;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
        projectileParticles.Pause(); // Stop the particle system

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtPlayer();

        //Boss Mouvement towards the player
        // Calculate the direction towards the player
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Apply a force towards the player
        rb.AddForce(direction * movementSpeed);

        // Clamp the speed so the boss doesn't move too fast
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, movementSpeed);


        if (APhase == BossAttackPhase.Phase1)
        {
            // First Phase of the boss attack
            timer += Time.deltaTime;
            if (timer >= timeInterval)
            {
                // Call your method here
                StartPhase(1);
                timer = 0f; // Reset the timer
            }

        }
        else if (APhase == BossAttackPhase.Phase2)
        {
            // second phase of boss attack
            if (!isIdentiSpawned)
            {
                StartPhase(2);
            }
        }
        else if (APhase == BossAttackPhase.Phase3)
        {
            // third phase of boss attack
            timer += Time.deltaTime;
            if (timer >= timeInterval)
            {
                StartPhase(3);
            }
        }
        else if (APhase == BossAttackPhase.Phase4)
        {
            // last phase of boss attack

            StartPhase(4);
        }

    }

    //Set the attack phase for the boss
    public void StartPhase(int phase)
    {
        switch (phase)
        {
            case 1:
                // Set up ranged attack pattern for phase 1
                StartCoroutine(RangedAttack());
                break;
            case 2:
                // Set up melee attack pattern for phase 2
                StartCoroutine(MeleeAttack());
                break;
            case 3:
                // Set up alternating attack pattern for phase 3
                StartCoroutine(AlternatingAttacks());
                break;
            case 4:
                // Set up special attack pattern for phase 4
                projectileParticles.Play(); // Play the particle system

                break;
            default:
                // Handle invalid phase number
                Debug.LogError("Invalid phase number: " + phase);
                break;
        }
    }

    // --Phase 1 projectile attack--
    IEnumerator RangedAttack()
    {
        bool hasSpawned = false;
        rb.velocity = Vector2.zero;

        while (true)
        {
            if (!hasSpawned)
            {
                // Choose a random spot on the screen to shoot towards
                Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y);

                // Calculate the direction to shoot towards the target position
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

                // Instantiate a projectile and shoot it in the calculated direction
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().velocity = direction * 5f;

                // Rotate the projectile to face the direction of its velocity
                projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, projectile.GetComponent<Rigidbody2D>().velocity);

                hasSpawned = true;
            }

            // Wait for a short period of time before shooting again
            yield return new WaitForSeconds(2f);
        }
    }

    // --Second Phase: charging attack--

    public int chargeSpeed = 10;
    public int knockbackForce = 50;
    public float meleeDamage = 10f;
    public float chargeCooldown;
    public GameObject chargeIndicatorPrefab;
    public AudioSource audioSource;
    public AudioClip chargingSound;

    IEnumerator MeleeAttack()
    {

        while (true)
        {
            // Set the boss's color to flashing red
            bossRenderer.material.color = Color.red;

            // Get the position of the player
            Vector3 playerPos = player.transform.position;

            // Spawn the charge indicator image at the player's position
            GameObject chargeIndicator = Instantiate(chargeIndicatorPrefab, playerPos, Quaternion.identity);

            isIdentiSpawned = true;

            // Wait for 1 second before charging
            yield return new WaitForSeconds(1f);

            // Charge towards the player's position
            Vector2 direction = (playerPos - transform.position).normalized;
            rb.velocity = direction * chargeSpeed;

            // Play the charging sound
            audioSource.PlayOneShot(chargingSound);

            // Wait until the boss has reached the player's position
            yield return new WaitUntil(() => Vector2.Distance(transform.position, playerPos) <= 0.1f);

            // Stop the boss's movement
            rb.velocity = Vector2.zero;

            // Check if the boss collided with the player
            if (isCollidingWithPlayer)
            {
                // Deal damage to the player
                player.GetComponent<PlayerHealth>().SetHealth(meleeDamage);

                // Knock back the player
                player.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            // Destroy the charge indicator image
            Destroy(chargeIndicator);

            isIdentiSpawned = false;

            // Reset the boss's color to white
            bossRenderer.material.color = Color.white;

            // Wait for the cooldown before attacking again
            yield return new WaitForSeconds(chargeCooldown);
        }
    }



    private bool isCollidingWithPlayer = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the boss is colliding with the player
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the boss is no longer colliding with the player
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }


    //--Third Phase: Alternating attack--

    IEnumerator AlternatingAttacks()
    {
        while (true)
        {
            // Alternate between ranged and melee attacks
            // Wait for a short period of time
            StartCoroutine(MeleeAttack());

            yield return new WaitForSeconds(2f);


            StartCoroutine(RangedAttack());
            yield return new WaitForSeconds(2f);

        }
    }

    public ParticleSystem projectileParticles; // Reference to the ParticleSystem component

    //  -- Fourth stage: special attack -- 

    // This method is called automatically when a particle collides with a collider
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player")) // Check if the particle collided with the player
        {
            Debug.Log("Particle damage");
            // Call a method on the player script to cause damage
            other.gameObject.GetComponent<PlayerHealth>().SetHealth(5f);
        }
    }

    // Rotate the boss towards the player
    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.transform.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.transform.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

}
