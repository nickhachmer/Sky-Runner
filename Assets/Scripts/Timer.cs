using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] private Text _timerText = default;
    [SerializeField] private GameState _gameState = default;
    private bool _running = default;

    public float ElapsedTime { get; private set; } = default;


    void Start()
    {
        ElapsedTime = _gameState.CurrentTime;
        _running = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_running)
        {
            ElapsedTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(ElapsedTime);
            _timerText.text = time.ToString("mm':'ss'.'ff");
        }
        
    }

    public void StartTimer()
    {
        _running = true;
    }

    // called when goal is reached
    public void StopTimer()
    {
        _gameState.CurrentTime = ElapsedTime;
        if (ElapsedTime < _gameState.BestTime || _gameState.BestTime == 0)
        {
            _gameState.BestTime = ElapsedTime;
        }
        _running = false;
    }
}
