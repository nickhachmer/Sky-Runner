using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private GameState _gameState = default;

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
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        switch (_gameState.CurrentScene) 
        {
            case 1: SceneManager.LoadScene("Part_1", LoadSceneMode.Additive); break;
            case 2: SceneManager.LoadScene("Part_2", LoadSceneMode.Additive); break;
            default:
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive); break;
        }
    }
}
