using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 10.0f;
    [SerializeField] float      m_rollForce = 12.0f;
    [SerializeField] bool       m_noBlood = false;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private bool                is_dead = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;

    float inputX, inputY;

    public GameObject Grave;
    public GameObject mainMenu;
    private GameObject m_bossInRange;



    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;


        // -- Handle input and movement --
        if (!is_dead) 
        { 
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling && !is_dead) 
        { 
            transform.position = transform.position + new Vector3(inputX * m_speed * Time.deltaTime, inputY * m_speed * Time.deltaTime, 0);
        }

        // -- Handle Animations --
        //Death
        if (GetComponent<PlayerHealth>().GetHealth() == 0f && !is_dead )
        {
            is_dead = true;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");

            Instantiate(Grave, transform);

            mainMenu.GetComponent<MainMenu>().EndGame();
         
        }

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling  && !is_dead)
        {
            AttackAndDealDamage();
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("space") && !m_rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
           
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon || Mathf.Abs(inputY) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0 && !is_dead)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Attack and damage dealing
    private void AttackAndDealDamage()
    {
        m_currentAttack++;

        // Loop back to one after third attack
        if (m_currentAttack > 3)
            m_currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);

        // Reset timer
        m_timeSinceAttack = 0.0f;

        // Dealing damage to the boss if there is one in range
        if (m_bossInRange != null)
        {
            m_bossInRange.GetComponent<BossHealth>().DealDamage(100);
            Debug.Log("Damage 100");
        }

        // Play attack sound effect
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
    }

    // Check for boss in range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            m_bossInRange = other.gameObject;
        }
    }

    // Clear boss in range
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            m_bossInRange = null;
        }
    }

    public void Knockback(Transform bossTransform, float knockbackForce)
    {
        // Calculate the direction from the boss to the player
        Vector3 knockbackDirection = (transform.position - bossTransform.position).normalized;

        // Apply the knockback force in the calculated direction to the player's rigidbody
        m_body2d.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    //Hurt animation
    public void DamageAnim() 
    {
        if (!is_dead)
        {
            m_animator.SetTrigger("Hurt");
        } 
    }

}


