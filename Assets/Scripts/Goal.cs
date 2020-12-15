using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
	[SerializeField] private Text _messageText = default;
	[SerializeField] private GameState _gameState = default;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			GameManager.Instance.Timer.StopTimer();
			_messageText.text = string.Format(
				"Best Time     {0}\nCurrent Time     {1}",
				_gameState.BestTime,
				_gameState.CurrentTime
			);
		}
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			_messageText.text = string.Empty;
		}
	}
}
