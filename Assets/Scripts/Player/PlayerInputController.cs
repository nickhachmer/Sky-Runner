using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{

    // This script will eventually handle all of the inputs when playing the game

    public GameObject pauseMenu;

    void Awake()
    {
        if (pauseMenu == null) {
            pauseMenu = GameObject.Find("PauseMenu");
        }
    }

    
    void Update()
    {
        
        bool pauseButtonPressed = Input.GetKeyDown(KeyCode.Escape);

        if (pauseButtonPressed) {
            pauseGame();
        }

        if (!pauseMenu.activeSelf) {
            resumeGame();
        }
    }


    private void pauseGame() {
        //might be better implemented on the GameManager
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);

    }

    private void resumeGame() {
        Time.timeScale = 1f;
    }

}
