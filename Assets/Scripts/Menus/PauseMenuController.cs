using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = default;
    [SerializeField] private GameObject _eventSystem = default;
    [SerializeField] private Timer _timer = default;
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
        _gameState.CurrentTime = _timer.ElapsedTime;
        _eventSystem.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(GameManager.Instance.SceneController.LoadMainMenu());
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
