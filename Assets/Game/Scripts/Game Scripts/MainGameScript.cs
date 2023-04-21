using UnityEngine;

public class MainGameScript : MonoBehaviour
{
    public GameObject Boss;
    private bool hasBeenCalled = false;
    public GameObject mainMenu;
    public GameObject bossBar;

    private void Start()
    {
        Time.timeScale = 0;
        mainMenu.SetActive(true);

    }

    public void BossSpawn()
    {
        if (!hasBeenCalled)
        {
            // Spawning the boss
            GameObject newBoss = Instantiate(Boss, transform.position, Quaternion.identity);
            hasBeenCalled = true;
            bossBar.SetActive(true);
        }
    }

    // Destroy game object
    public void DestroyGameObject(GameObject obj, float length)
    {
        Destroy(obj, length);
    }
}
