using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;  // Import the UnityEditor namespace

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playerUI;
    public GameObject bossBar;

    private bool isgameOver = false;


    public void Update()
    {
       
    }

    // Start a new game
    public void StartNew()
    {
        if (!isgameOver)
        {
            Time.timeScale = 1;
            mainMenu.SetActive(false);
            playerUI.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("BossFight");
            Time.timeScale = 1;
            mainMenu.SetActive(false);
            playerUI.SetActive(true);
        }

    }

    public void EndGame()
    {
        isgameOver = true;
        Time.timeScale = 0;
        mainMenu.SetActive(true);
        playerUI.SetActive(false);
        bossBar.SetActive(false);
    }

    // Quit game button
    public void QuitGame()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;  // Stop play mode
        }
        else
        {
            Application.Quit();  // Quit the Editor
        }
        
    }
}
