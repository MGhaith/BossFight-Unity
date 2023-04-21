using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthValueScript : MonoBehaviour
{
    public TMP_Text playerHealth;
    public TMP_Text attackPhase;
    private GameObject player;
    private GameObject boss;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(attackPhase != null)
        {
            attackPhase.text = "";
        } 

    }

    // Update is called once per frame
    void Update()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        if (playerHealth != null && player != null)
        {
            playerHealth.text = "" + player.GetComponent<PlayerHealth>().GetHealth();
        }

        if (attackPhase != null && boss != null)
        {
            attackPhase.text = "" + boss.GetComponent<BossController>().APhase;
        }
    }
}
