using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] private Text _timerText = default;
    [SerializeField] private GameState _gameState = default;
    private float _elapsedTime = default;
    private bool _running = default;

    void Start()
    {
        _running = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_running)
        {
            _elapsedTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(_elapsedTime);
            _timerText.text = time.ToString("mm':'ss'.'ff");
        }
        
    }

    public void StartTimer()
    {
        _elapsedTime = 0f;
        _running = true;
    }

    public void StopTimer()
    {
        _gameState.BestTime = _elapsedTime;
        _running = false;
    }

}
