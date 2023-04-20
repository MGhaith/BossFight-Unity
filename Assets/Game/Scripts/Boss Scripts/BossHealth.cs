using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
	public int bossHealth = 1000;
	public AudioSource audioSource;
	public AudioClip bossdeathSound;


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

	public void DealDamage(int damage)
	{
		
		if (bossHealth <= 0)
		{
			Die();
		}
        else 
		{
			bossHealth -= damage;

			StartCoroutine(DamageAnimation());
		}
	}

	void Die()
	{
		GameObject gc = GameObject.FindGameObjectWithTag("GameController");
		audioSource.PlayOneShot(bossdeathSound);
		gc.GetComponent<MainGameScript>().DestroyGameObject(this.gameObject, bossdeathSound.length);
		
	}

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
