using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = new GameManager();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    GameManager() {  }

}
