using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int bossHealth;
    public int maxBossHealth = 1000;
    public AudioSource audioSource;
    public AudioClip bossdeathSound;
    public HealthbarScript healthBar; // public refrence for the health bar

    private bool isBossDestroyed = false;

    private void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HManager").GetComponent<HealthbarScript>();
        Debug.Log(healthBar);
        bossHealth = maxBossHealth;
        healthBar.SetMaxHealth(maxBossHealth);

    }

    private void Update()
    {
        // Check boss health and update phase accordingly
        if (bossHealth <= 750 && GetComponent<BossController>().APhase == BossController.BossAttackPhase.Phase1)
        {
            GetComponent<BossController>().APhase = BossController.BossAttackPhase.Phase2;
        }
        else if (bossHealth <= 500 && GetComponent<BossController>().APhase == BossController.BossAttackPhase.Phase2)
        {
            GetComponent<BossController>().APhase = BossController.BossAttackPhase.Phase3;
        }
        else if (bossHealth <= 250 && GetComponent<BossController>().APhase == BossController.BossAttackPhase.Phase3)
        {
            GetComponent<BossController>().APhase = BossController.BossAttackPhase.Phase4;
        }
    }

    // Dealing damage to the boss
    public void DealDamage(int damage)
    {

        if (bossHealth <= 0)
        {
            Die();
            //endgame
        }
        else
        {
            bossHealth -= damage;
            healthBar.SetHealth(bossHealth);
            StartCoroutine(DamageAnimation());
        }
    }


    // Destroying the boss & playing death sound
    void Die()
    {
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");
        audioSource.PlayOneShot(bossdeathSound);
        while (!isBossDestroyed)
        {
            gc.GetComponent<MainGameScript>().DestroyGameObject(this.gameObject, bossdeathSound.length);

            isBossDestroyed = true;
        }
        GetComponent<BossController>().player.GetComponent<HeroKnight>().mainMenu.GetComponent<MainMenu>().EndGame();
    }


    // Flashing damage effect
    IEnumerator DamageAnimation()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < 3; i++)
        {
            foreach (SpriteRenderer sr in srs)
            {
                Color c = sr.color;
                c.a = 0;
                sr.color = c;
            }

            yield return new WaitForSeconds(.1f);

            foreach (SpriteRenderer sr in srs)
            {
                Color c = sr.color;
                c.a = 1;
                sr.color = c;
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
