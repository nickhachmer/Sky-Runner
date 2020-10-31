using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    
    private PlayerMovementController _playerMovementController = default;
    private PauseMenuBehavior pauseMenu = default;

    public static bool isGamePaused;
    private static GameManager _instance;
    public static GameManager Instance { 
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }

    public GameManager()
    {
        _playerMovementController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
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

    public PlayerMovementController getPlayerMovementController()
    {
        return _playerMovementController;
    }
}
