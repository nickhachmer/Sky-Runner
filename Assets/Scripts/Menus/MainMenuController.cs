using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public GameObject _eventSystem = default;

    [SerializeField] private GameState _gameState = default;
    [SerializeField] private GameObject _playFromCheckpointButton = default;
    [SerializeField] private Text _bestTimeText = default;
    private bool isBestTimeTextShowing = false;

    void Start()
    {
        if (_gameState.Checkpoint == new Vector3(-6, -4.45f, -1))
        {
            _playFromCheckpointButton.SetActive(false);
        } else
        {
            _playFromCheckpointButton.SetActive(true);
        }
    }

    public void PlayFromCheckpoint()
    {
        LoadGame();
    }

    public void PlayGame()
    {
        _gameState.Checkpoint = new Vector3(-6, -4.45f, -1);
        _gameState.CurrentTime = 0f;
        LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleBestTimeText()
    {
        if (isBestTimeTextShowing)
        {
            _bestTimeText.text = string.Empty;
            isBestTimeTextShowing = false;
        } 
        else
        {
            TimeSpan bestTime = TimeSpan.FromSeconds(_gameState.BestTime);
            _bestTimeText.text = string.Format(
                "Best Time : {0}",
                bestTime.ToString("mm':'ss'.'ff")
            );
            isBestTimeTextShowing = true;
        }
    }

    private void LoadGame()
    {
        _eventSystem.SetActive(false);
        StartCoroutine(GameManager.Instance.SceneController.LoadGame());
    }
}
