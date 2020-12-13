using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{

    private static GameManager _instance;

    private PlayerController _playerController;
    private CameraMovementController _cameraMovementController;
    private SceneController _sceneController;
    private PauseMenuController _pauseMenu;    

    public PlayerController PlayerController { 
        get
        {
            if (_playerController == null)
            {
                _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            }

            return _playerController;
        }
    }

    public CameraMovementController CameraMovementController
    {
        get
        {
            if (_cameraMovementController == null)
            {
                _cameraMovementController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovementController>();
            }

            return _cameraMovementController;
        }
    }
    
    public SceneController SceneController
    {
        get
        {
            if (_sceneController == null)
            {
                _sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
            }

            return _sceneController;
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
