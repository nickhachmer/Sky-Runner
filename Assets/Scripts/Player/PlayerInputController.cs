using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    void Awake()
    {
    }

    
    void Update()
    {
        
        bool pauseButtonPressed = Input.GetKeyDown(KeyCode.Escape);

        if (pauseButtonPressed && !GameManager.isGamePaused) {
            GameManager.PauseGame();
        } else if (pauseButtonPressed && GameManager.isGamePaused) {
            GameManager.ResumeGame();
        }
    }

}
