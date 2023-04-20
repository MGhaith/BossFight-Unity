using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameScript : MonoBehaviour
{
    public GameObject Boss;
    private bool hasBeenCalled = false;

    public void BossSpawn() 
    {
        if (!hasBeenCalled)
        {
            // Spawning the boss
            GameObject newBoss = Instantiate(Boss, transform.position, Quaternion.identity);
            Debug.Log("Boss Spawning");
            hasBeenCalled = true;
        }
    }

    // Destroy game object
    public void DestroyGameObject(GameObject obj, float length )
    {
        Destroy(obj, length);
    }
}
