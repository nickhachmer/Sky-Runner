using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuController
{
    
    public static void Transition(GameObject from, GameObject to) {
        from.SetActive(false);
        to.SetActive(true);
    }

    public static void LoadScene(int SceneBuildIndex) {
        // not sure if I need to unload current scene - will look into
        SceneManager.LoadScene(SceneBuildIndex);
    }

}
