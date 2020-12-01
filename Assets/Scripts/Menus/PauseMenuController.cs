using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu = default;
    [SerializeField] private GameObject OptionsMenu = default;

    void Start()
    {
        GameManager.Instance.PauseMenu = gameObject.GetComponent<PauseMenuController>();
        hidePauseMenu();
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }


    public void showPauseMenu()
    {
        PauseMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void hidePauseMenu()
    {
        PauseMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }
}
