using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (gameObject.activeSelf)
			{
				GameManager.Instance.Timer.StopTimer();
			}
		}
	}
}
