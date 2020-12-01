using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{

    private static GameManager _instance;

    private PlayerMovementController _playerMovementController;
    private PauseMenuController _pauseMenu;    

    public PlayerMovementController PlayerMovementController { 
        get
        {
            if (_playerMovementController == null)
            {
                _playerMovementController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
            }

            return _playerMovementController;
        }
    }

    public PauseMenuController PauseMenu 
    {
        get
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenuController>();
                if (_pauseMenu == null) Debug.Log("empty");
            }

            return _pauseMenu;
        }
        set
        {
            _pauseMenu = value;
        }
    }

    public static bool isGamePaused = false;

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

    public GameManager() { }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        PauseMenu.showPauseMenu();
    }

    public void ResumeGame()
    {
        PauseMenu.hidePauseMenu();
        Time.timeScale = 1f;
        isGamePaused = false;
        
    }

}
