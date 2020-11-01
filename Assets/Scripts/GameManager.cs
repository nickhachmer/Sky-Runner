using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    
    private PlayerMovementController _playerMovementController = default;
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

    public PlayerMovementController getPlayerMovementController()
    {
        return _playerMovementController;
    }
}
