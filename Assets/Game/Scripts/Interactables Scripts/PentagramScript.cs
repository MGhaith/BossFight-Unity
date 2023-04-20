using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentagramScript : MonoBehaviour
{

    public AudioSource audioSource; // assign in the Inspector
    public AudioClip audioClip; // assign in the Inspector

    // Start is called before the first frame update
    void Start()
    {
      
    }

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D cc)
    {
        // Only run the code if the method hasn't been called before
        if (!hasTriggered)
        {
            hasTriggered = true;

            GameObject gc = GameObject.FindGameObjectWithTag("GameController");
            gc.GetComponent<MainGameScript>().BossSpawn();

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip);

            gc.GetComponent<MainGameScript>().DestroyGameObject(this.gameObject, audioClip.length);
        }
    }
}
