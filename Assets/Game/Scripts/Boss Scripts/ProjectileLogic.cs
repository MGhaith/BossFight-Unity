using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public float projectileDamage = 10f;

    private void Start()
    {

        // Destroy the projectile after a certain amount of time
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the projectile forward in its local space
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collider2D playerBody = player.GetComponent<BoxCollider2D>();
       
        if (other == playerBody)
        {
            //Projectile collides with the player
            player.GetComponent<PlayerHealth>().SetHealth(projectileDamage);
            Debug.Log("Player Hit");
            Destroy(this.gameObject);

        }       
    }
}
