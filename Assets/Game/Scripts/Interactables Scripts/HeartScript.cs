using UnityEngine;

public class HeartScript : MonoBehaviour
{
    public AudioSource audioSource; // assign in the Inspector
    public AudioClip audioClip; // assign in the Inspector
    public Vector3 stationPos;

    public float spawnInterval = 10f; // The time interval between each heart spawn
    private float timeSinceLastSpawn = 0f; // The time since the last heart spawn

    // Update is called once per frame
    public void SpawnHeart()
    {
        // Increment the time since the last spawn
        timeSinceLastSpawn += Time.deltaTime;

        // Check if it's time to spawn a heart
        if (timeSinceLastSpawn >= spawnInterval)
        {
            // Reset the time since the last spawn
            timeSinceLastSpawn = 0f;

            // Spawn a new heart prefab at the player's position
            Instantiate(this.gameObject, stationPos, Quaternion.identity);
        }

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

