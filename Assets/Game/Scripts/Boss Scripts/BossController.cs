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


    private float timer = 0f;
    public float timeInterval = 10f; // Interval to call the method
    private SpriteRenderer bossRenderer;
    bool isMeleeAttack = false;
    bool isAltAttack = false;



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
    void Update()
    {
        LookAtPlayer();

        
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
            StartPhase(2);
        }
        else if (APhase == BossAttackPhase.Phase3)
        {
            // third phase of boss attack
            isMeleeAttack = false;
            StartPhase(3);

        }
        else if (APhase == BossAttackPhase.Phase4)
        {
            // last phase of boss attack
            isAltAttack = false;
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
                if (!isMeleeAttack) // Check if the coroutine is not already running
                {
                    StopAllCoroutines();
                    StartCoroutine(MeleeAttack());

                }
                break;
            case 3:
                // Set up melee attack pattern for phase 2
                if (!isAltAttack) // Check if the coroutine is not already running
                {
                    StopAllCoroutines();
                    // Set up alternating attack pattern for phase 3
                    StartCoroutine(AlternatingAttacks());
                }
                break;
            case 4:
                // Set up special attack pattern for phase 4
                StopAllCoroutines();
                transform.position = new Vector3(0f, 0f, 0f);
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
    public int knockbackForce = 5;
    public float meleeDamage = 10f;
    public float chargeCooldown;
    public GameObject chargeIndicatorPrefab;
    public AudioSource audioSource;
    public AudioClip chargingSound;

    IEnumerator MeleeAttack()
    {

        while (true)
        {
            // Set the flag to indicate that the coroutine is running
            isMeleeAttack = true;

            // Set the boss's color to flashing red
            bossRenderer.material.color = Color.red;

            // Get the position of the player
            Vector3 playerPos = player.transform.position;

            //check if there is any non-destroyed charge indicators
            GameObject[] Junk = GameObject.FindGameObjectsWithTag("target");

            if (Junk.Length != 0)
            {
                foreach (GameObject c in Junk)
                {
                    Destroy(c);
                }
            }

            // Spawn the charge indicator image at the player's position
            GameObject chargeIndicator = Instantiate(chargeIndicatorPrefab, playerPos, Quaternion.identity);


            // Charge towards the player's position
            Vector2 direction = (playerPos - transform.position).normalized;
            rb.velocity = direction * chargeSpeed;

            // Play the charging sound
            audioSource.PlayOneShot(chargingSound);

            // Save the start time
            float startTime = Time.time;

            // Wait until the boss has reached the player's position
            yield return new WaitUntil(() => Vector2.Distance(transform.position, playerPos) <= 0.1f || Time.time > startTime + 1f);

            // Check if the boss collided with the player
            if (isCollidingWithPlayer)
            {
                // Deal damage to the player
                player.GetComponent<PlayerHealth>().SetHealth(meleeDamage);

                // Knock back the player
                player.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);

                // If the timeout has been reached, stop the boss's movement
                rb.velocity = Vector2.zero;
            }

            else if (Time.time > startTime + 1f)
            {
                // If the timeout has been reached, stop the boss's movement
                rb.velocity = Vector2.zero;
            }

            // Reset the boss's color to white
            bossRenderer.material.color = Color.white;

            // Wait for the cooldown before attacking again
            yield return new WaitForSeconds(chargeCooldown);


            // Destroy the charge indicator image
            Destroy(chargeIndicator);

            // Set the flag to indicate that the coroutine has finished running
            isMeleeAttack = false;

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
            // Set the flag to indicate that the coroutine is running
            isAltAttack = true;

            rb.velocity = Vector2.zero;

            // Choose a random spot on the screen to shoot towards
            Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y);

            // Calculate the direction to shoot towards the target position
            Vector2 directionProjec = (targetPosition - (Vector2)transform.position).normalized;

            // Shoot 4 projectiles in a circular pattern around the boss
            for (int i = 0; i < 4; i++)
            {
                // Calculate the direction to shoot each projectile
                float angle = i * 90f;
                Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * directionProjec;

                // Instantiate a projectile and shoot it in the calculated direction
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().velocity = rotatedDirection * 10f;

                // Rotate the projectile to face the direction of its velocity
                projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, projectile.GetComponent<Rigidbody2D>().velocity);
            }

            yield return new WaitForSeconds(2f);

            // Melee Attack
            // Set the boss's color to flashing red
            bossRenderer.material.color = Color.red;

            // Get the position of the player
            Vector3 playerPos = player.transform.position;

            //check if there is any non-destroyed charge indicators
            GameObject[] Junk = GameObject.FindGameObjectsWithTag("target");

            if (Junk.Length != 0)
            {
                foreach (GameObject c in Junk)
                {
                    Destroy(c);
                }
            }

            // Spawn the charge indicator image at the player's position
            GameObject chargeIndicator = Instantiate(chargeIndicatorPrefab, playerPos, Quaternion.identity);

            // Charge towards the player's position
            Vector2 direction = (playerPos - transform.position).normalized;
            rb.velocity = direction * chargeSpeed;

            // Play the charging sound
            audioSource.PlayOneShot(chargingSound);

            // Save the start time
            float startTime = Time.time;

            // Wait until the boss has reached the player's position
            yield return new WaitUntil(() => Vector2.Distance(transform.position, playerPos) <= 0.1f || Time.time > startTime + 1f);

            // Check if the boss collided with the player
            if (isCollidingWithPlayer)
            {
                // Deal damage to the player
                player.GetComponent<PlayerHealth>().SetHealth(meleeDamage);

                // If the timeout has been reached, stop the boss's movement
                rb.velocity = Vector2.zero;
            }     

            else if(Time.time > startTime + 1f)
            {
                // If the timeout has been reached, stop the boss's movement
                rb.velocity = Vector2.zero;
            }

            // Knock back the player
            player.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);

            // Reset the boss's color to white
            bossRenderer.material.color = Color.white;

            // Wait for the cooldown before attacking again
            yield return new WaitForSeconds(chargeCooldown);

            // Destroy the charge indicator image
            Destroy(chargeIndicator);

            // Set the flag to indicate that the coroutine has finished running
            isAltAttack = false;
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
