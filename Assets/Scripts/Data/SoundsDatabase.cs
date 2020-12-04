using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsDatabase", menuName = "Databases/SoundsDatabase")]
public class SoundsDatabase : ScriptableObject
{
	[SerializeField] private AudioClip _jump = default;
	[SerializeField] private AudioClip _dash = default;
	[SerializeField] private AudioClip _wallJump = default;
	[SerializeField] private AudioClip _onWall = default;
	[SerializeField] private AudioClip _orb = default;
	[SerializeField] private AudioClip _movingFast = default;

	public AudioClip Jump => _jump;
	public AudioClip Dash => _dash;
	public AudioClip WallJump => _wallJump;
	public AudioClip OnWall => _onWall;
	public AudioClip Orb => _orb;
	public AudioClip MovingFast => _movingFast;
}
