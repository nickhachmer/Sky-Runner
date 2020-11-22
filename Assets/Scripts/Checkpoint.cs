using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private Vector3 _spawnPosition = default;
	[SerializeField] private GameState _gameState = default;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (gameObject.activeSelf)
			{
				_gameState.Checkpoint = _spawnPosition;
				gameObject.SetActive(false);
			}
		}
	}
}