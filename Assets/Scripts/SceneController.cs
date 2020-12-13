using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator LoadGame(string sceneToLoad)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncOperation.isDone);

        if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            Debug.Log("herhe rhaehr heahr aher");
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
            yield return new WaitUntil(() => asyncOperation.isDone);
        }

        if (SceneManager.GetSceneByName("MainMenu").isLoaded)
        {
            asyncOperation = SceneManager.UnloadSceneAsync("MainMenu");
            yield return new WaitUntil(() => asyncOperation.isDone);
        }
    }

    public IEnumerator LoadMainMenu()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);

        yield return new WaitWhile(() => !asyncOperation.isDone);

        if (SceneManager.GetSceneByName("Tutorial").isLoaded)
        {
            asyncOperation = SceneManager.UnloadSceneAsync("Tutorial");
            yield return new WaitUntil(() => asyncOperation.isDone);
        }

        if (SceneManager.GetSceneByName("Part_1").isLoaded)
        {
            asyncOperation = SceneManager.UnloadSceneAsync("Part_1");
            yield return new WaitUntil(() => asyncOperation.isDone);
        }

        if (SceneManager.GetSceneByName("Part_2").isLoaded)
        {
            asyncOperation = SceneManager.UnloadSceneAsync("Part_2");
            yield return new WaitUntil(() => asyncOperation.isDone);
        }

        if (SceneManager.GetSceneByName("Gameplay").isLoaded)
        {
            asyncOperation = SceneManager.UnloadSceneAsync("Gameplay");
            yield return new WaitUntil(() => asyncOperation.isDone);
        }
        
    }
}
