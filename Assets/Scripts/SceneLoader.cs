// Source/References: https://github.com/amel-unity/Multi-Scene-workflow/blob/master/Assets/Creator%20Kit%20-%20FPS/Scripts/System/ScenePartLoader.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Transform _player = default;
    [SerializeField] private float _loadRange = default;

    private bool _isLoaded = false;
    
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
        DistanceCheck();
    }

    private void DistanceCheck()
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

    private void LoadScene()
    {
        if (!_isLoaded)
        {
            // Loading the scene, using the gameobject name as it's the same as the name of the scene to load
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            // We set it to true to avoid loading the scene twice
            _isLoaded = true;
        }
    }

    private void UnLoadScene()
    {
        if (_isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            _isLoaded = false;
        }
    }
}
