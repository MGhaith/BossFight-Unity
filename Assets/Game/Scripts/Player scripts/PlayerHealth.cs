using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	
	public bool isImmortal = false;
	public AudioClip gruntSound;
	public AudioSource audioSource;
	
	private float       hp = 100f;

    // health getter
    public float GetHealth() 
    {
        return hp;
    }

	// health damage
	public void SetHealth(float damage)
	{
        if (hp > 0 && !isImmortal) 
		{
			hp -= damage;
			GetComponent<HeroKnight>().DamageAnim();
			audioSource.PlayOneShot(gruntSound);
			Debug.Log("Health =" + hp);
		}
	}

	// Add Health
	public void AddHealth(float BonusHealth)
	{
        if (hp <= 100f )
        {
			hp += BonusHealth;
			if (hp > 100f)
			{
				hp = 100f;
			}

		}
	}
	
}
