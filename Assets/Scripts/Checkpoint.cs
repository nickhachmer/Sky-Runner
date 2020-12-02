using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private Vector3 _spawnPosition = default;
	[SerializeField] private GameState _gameState = default;
	[SerializeField] private int _heightLimit = default;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (gameObject.activeSelf)
			{
				_gameState.Checkpoint = _spawnPosition;
				_gameState.HeightLimit = _heightLimit;
				gameObject.SetActive(false);
			}
		}
	}
}