using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    public AudioSource audioSource; // assign in the Inspector
    public AudioClip audioClip; // assign in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D cc)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Only run the code if the method hasn't been called before
        if (!hasTriggered && cc.gameObject == player)
        {
            hasTriggered = true;

            player.GetComponent<PlayerHealth>().AddHealth(20f);

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip);

            Debug.Log("Health +20");

            GameObject gc = GameObject.FindGameObjectWithTag("GameController");
            gc.GetComponent<MainGameScript>().DestroyGameObject(this.gameObject, audioClip.length);
        }
    }

}

