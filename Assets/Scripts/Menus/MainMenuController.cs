using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private GameState _gameState = default;

    public void PlayFromCheckpoint()
    {
        LoadGame();
    }

    public void PlayGame()
    {
        _gameState.Checkpoint = Vector3.zero;
        LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void LoadGame()
    {

    }
}
