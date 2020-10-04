using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager instance;
    private static PauseMenuBehavior pauseMenu;
    public static bool isGamePaused;
    

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public void PauseGame() {
        Time.timeScale = 0f;
        isGamePaused = true;
        pauseMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenuBehavior>();
        if (pauseMenu != null) {
            pauseMenu.showPauseMenu();
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        isGamePaused = false;
        if (pauseMenu != null) {
            pauseMenu.hidePauseMenu();
        }
    }

    // I want to use game manager as a way for scripts to communicate with eachother with out having to get eachothers references

    // Also I will have game options in here
}
