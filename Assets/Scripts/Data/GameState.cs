using System;
using UnityEngine;

// the model for the game
[CreateAssetMenu(fileName = "GameState", menuName = "Databases/GameState")]
public class GameState : ScriptableObject
{
	public event Action OnUpdateGameState;

	[SerializeField] private Vector3 _checkpoint = Vector3.zero;
	[SerializeField] private int _currentScene = 0;

	public Vector3 Checkpoint 
	{
		get
		{
			return _checkpoint;
		}

		set
		{
			_checkpoint = value;
			OnUpdateGameState?.Invoke();
		}
	}

	public int CurrentScene 
	{
        get
        {
			return _currentScene;
        }	

		set
        {
			_currentScene = value;
        }
	}

}