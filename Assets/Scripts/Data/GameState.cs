using System;
using UnityEngine;

// the model for the game
[CreateAssetMenu(fileName = "GameState", menuName = "Databases/GameState")]
public class GameState : ScriptableObject
{
	public event Action OnUpdateGameState;

	[SerializeField] private Vector3 _checkpoint = new Vector3(-6, -4.45f, -1);
	[SerializeField] private float _currentTime = 0;
	[SerializeField] private float _bestTime = 0;

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

	public float BestTime
	{
		get
		{
			return _bestTime;
		}

		set
		{
			if (value < _bestTime || _bestTime == 0)
            {
				_bestTime = value;
			}
		}
	}
	
	public float CurrentTime
	{
		get
		{
			return _currentTime;
		}

		set
		{
			_currentTime = value;
		}
	}
}