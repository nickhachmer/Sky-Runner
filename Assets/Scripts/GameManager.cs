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

    // I want to use game manager as a way for scripts to communicate with eachother with out having to get eachothers references

    // Also I will have game options in here
}
