// Source/References: https://github.com/amel-unity/Multi-Scene-workflow/blob/master/Assets/Creator%20Kit%20-%20FPS/Scripts/System/ScenePartLoader.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public enum CheckMethod
{
    Distance,
    Trigger
}

public class SceneLoader : MonoBehaviour
{

    public CheckMethod _checkMethod;
    [SerializeField] private Transform _player = default;
    [SerializeField] private float _loadRange = default;

    // Scene state
    private bool _isLoaded;
    private bool _shouldLoad;
    void Start()
    {
        // verify if the scene is already open to avoid opening a scene twice
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name)
                {
                    _isLoaded = true;
                }
            }
        }
    }

    void Update()
    {
        // Checking which method to use
        if (_checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else if (_checkMethod == CheckMethod.Trigger)
        {
            TriggerCheck();
        }
    }

    void DistanceCheck()
    {
        // Checking if the player is within the range
        if (Vector3.Distance(_player.position, transform.position) < _loadRange)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    void LoadScene()
    {
        if (!_isLoaded)
        {
            // Loading the scene, using the gameobject name as it's the same as the name of the scene to load
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            // We set it to true to avoid loading the scene twice
            _isLoaded = true;
        }
    }

    void UnLoadScene()
    {
        if (_isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            _isLoaded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _shouldLoad = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _shouldLoad = false;
        }
    }

    void TriggerCheck()
    {
        // _shouldLoad is set from the Trigger methods
        if (_shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }
}
