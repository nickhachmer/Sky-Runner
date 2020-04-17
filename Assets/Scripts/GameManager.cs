using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static PauseMenuBehavior pauseMenu;
    public static bool isGamePaused;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = new GameManager();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    GameManager() {  }

    public static void PauseGame() {
        Time.timeScale = 0f;
        isGamePaused = true;
        pauseMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenuBehavior>();
        if (pauseMenu != null) {
            pauseMenu.showPauseMenu();
        }
    }

    public static void ResumeGame() {
        Time.timeScale = 1f;
        isGamePaused = false;
        if (pauseMenu != null) {
            pauseMenu.hidePauseMenu();
        }
    }

    // I want to use game manager as a way for scripts to communicate with eachother with out having to get eachothers references

    // Also I will have game options in here
}
