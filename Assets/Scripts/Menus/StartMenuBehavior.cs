using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuBehavior : MonoBehaviour
{
 
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject LevelSelect;

    void Awake() {
        if (MainMenu == null) {
            MainMenu = GameObject.Find("MainMenu");
        }
        if (OptionsMenu == null) {
            MainMenu = GameObject.Find("OptionsMenu");
        }
        if (LevelSelect == null) {
            MainMenu = GameObject.Find("LevelSelect");
        }
    }

    void Start() {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        LevelSelect.SetActive(false);
    }


    // In the editor the buttons are set to call these functions when they have been pressed


    #region Main Menu

    public void MainMenu_Button_Play() {
        MenuController.Transition(MainMenu, LevelSelect);
    }

    public void MainMenu_Button_Options() {
        MenuController.Transition(MainMenu, OptionsMenu);
    }

    public void MainMenu_Button_Quit() {
        Application.Quit();
    }

    #endregion

    #region Options Menu

    public void OptionsMenu_Button_Back() {
        MenuController.Transition(OptionsMenu, MainMenu);
    }

    #endregion

    #region Level Select

    public void LevelSelect_Button_Play(int levelNumber) {
        MenuController.LoadScene(levelNumber);
    }

    public void LevelSelect_Button_Back() {
        MenuController.Transition(LevelSelect, MainMenu);
    }

    #endregion

    


}
