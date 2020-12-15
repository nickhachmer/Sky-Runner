using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Databases/GameSettings")]
public class GameSettings : ScriptableObject
{
	public event Action OnUpdateGameSettings;

	[SerializeField] private float _soundEffectsVolume = default;
	[SerializeField] private float _windVolume = default;

	public float SoundEffectsVolume
	{
		get
		{
			return _soundEffectsVolume;
		}

		set
		{
			if (value >= 1)
            {
				_soundEffectsVolume = 1;
			} 
			else
            {
				_soundEffectsVolume = value;
			}
			
			OnUpdateGameSettings?.Invoke();
		}
	}

	public float WindVolume
	{
		get
		{
			return _windVolume;
		}

		set
		{
			if (value >= 1)
			{
				_windVolume = 1;
			}
			else
			{
				_windVolume = value;
			}
			OnUpdateGameSettings?.Invoke();
		}
	}
}
