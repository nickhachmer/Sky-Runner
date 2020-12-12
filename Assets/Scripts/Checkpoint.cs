using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private ParticleSystem _paticles = default;
	[SerializeField] private GameState _gameState = default;
	[SerializeField] private int _currentScene = default;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (gameObject.activeSelf)
			{
				_gameState.Checkpoint = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, -1);
				_gameState.CurrentScene = _currentScene;
				_paticles.Play();
			}
		}
	}
}