using System;
using UnityEngine;

// the model for the game
[CreateAssetMenu(fileName = "GameState", menuName = "Databases/GameState")]
public class GameState : ScriptableObject
{
	public event Action OnUpdateGameState;

	[SerializeField] private Vector3 _checkpoint = default;
	[SerializeField] private int _heightLimit = default;
	// TODO: Add field for the current scene

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

	public int HeightLimit
	{
		get
		{
			return _heightLimit;
		}

		set
		{
			_heightLimit = value;
			OnUpdateGameState?.Invoke();
		}
	}
}