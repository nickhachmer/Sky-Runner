using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "PhysicsDatabase", menuName = "Databases/PhysicsDatabase")]
public class PhysicsDatabase : ScriptableObject
{
	// NOTE: should eventually refactor most of these values into a set of global constants
	[SerializeField] private float _gravityScale = default;
	[SerializeField] private float _playerMass = default;
	[SerializeField] private float _playerAngularDrag = default;
	[SerializeField] private float _playerLinearDrag = default;
	[SerializeField] private int _playerSpeed = default;
	[SerializeField] private int _playerJumpForce = default;
	[SerializeField] private int _playerDownForce = default;
	[SerializeField] private int _playerVerticalComponentDashForce = default;
	[SerializeField] private int _orbForceMultiplier = default;
	[SerializeField] private short _orbRange = default;
	

	public float GravityScale => _gravityScale;
	public float PlayerMass => _playerMass;
	public float PlayerAngularDrag => _playerAngularDrag;
	public float PlayerLinearDrag => _playerLinearDrag;
	public int PlayerSpeed => _playerSpeed;
	public int PlayerJumpForce => _playerJumpForce;
	public int PlayerDownForce => _playerDownForce;
	public int PlayerVerticalComponentDashForce => _playerVerticalComponentDashForce;
	public int OrbForceMultiplier => _orbForceMultiplier;
	public short OrbRange => _orbRange;

}