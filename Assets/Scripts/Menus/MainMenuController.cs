using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject es = default;

    [SerializeField] private GameState _gameState = default;
    [SerializeField] private GameObject _playFromCheckpointButton = default;

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
        LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void LoadGame()
    {
        es.SetActive(false);
        StartCoroutine(GameManager.Instance.SceneController.LoadGame());
    }
}
