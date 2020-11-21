﻿using System;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Player;

public class PlayerMovementController : MonoBehaviour
{
    #region PlayerMovement Properties

    // public player values set in editor
    [SerializeField] private BoxCollider2D              _boxCollider = default;
    [SerializeField] private Rigidbody2D                _rigidBody = default;
    [SerializeField] private Transform                  _transform = default;
    [SerializeField] private PlayerAnimationController  _animation = default;
    [SerializeField] private PhysicsDatabase            _physicsDatabase = default;
    [SerializeField] private LayerMask                  _terrainLayer = default;
    [SerializeField] private LayerMask                  _harmLayer = default;
    [SerializeField] private ParticleSystem             _dashParticles = default;
    [SerializeField] private ParticleSystem             _walkParticles = default;
    [SerializeField] private int                        _maxJumps = default;
    [SerializeField] private int                        _yPositionLimit = default;
    [SerializeField] private float                      _stretchConstant = default;
    
    public Transform Transform => _transform;
    public InputMaster _controls;
    private PlayerState _playerState = default;
    
    private Action<bool, bool, bool, bool> UpdateAnimationState = default;
    private Action DeathAnimation = default;

    private int _speed = default; 
    private int _jumpForce = default; 
    private int _downForce = default;
    private int _verticalComponentDashForce = default;
    private float _gravityScale = default; 
    private int _orbForceMultiplier = default;
    private Vector3 _activeOrbPosition = new Vector3(0, 0, 0);
    private Vector3 _respawnPosition = default;

    private float _horizontalAxis = 0;
    private float _verticalAxis = 0;

    #endregion

    private void Awake() {

        _rigidBody.gravityScale = _gravityScale;
        _rigidBody.angularDrag = _physicsDatabase.PlayerAngularDrag;
        _rigidBody.drag = _physicsDatabase.PlayerLinearDrag;
        _rigidBody.mass = _physicsDatabase.PlayerMass;

        _speed = _physicsDatabase.PlayerSpeed;
        _jumpForce = _physicsDatabase.PlayerJumpForce;
        _downForce = _physicsDatabase.PlayerDownForce;
        _verticalComponentDashForce = _physicsDatabase.PlayerVerticalComponentDashForce;
        _gravityScale = _physicsDatabase.GravityScale;
        _orbForceMultiplier = _physicsDatabase.OrbForceMultiplier;
        _respawnPosition = Vector3.zero;

        UpdateAnimationState += _animation.SetAnimationState;
        DeathAnimation += _animation.SetDeathState;

        _controls = new InputMaster();
        _controls.Player.Orb.canceled += context =>
        {
            _playerState.slingShotReleased = true;
        };

        
    }
    
    private void Start()
    {
        _playerState.currentDirectionFacing = DirectionFacing.Right;
        _playerState.currentMovementState = MovementState.Default;
    }

    /** 
    * Capture inputs and determine current movement state
    */
    private void Update()
    {

        // stretches the player sprite based on its vertical velocity
        _transform.localScale = new Vector3(4 + _rigidBody.velocity.x/ _stretchConstant, 4 - _rigidBody.velocity.y / _stretchConstant, 1);

        if (_transform.position.y < _yPositionLimit && !_playerState.isDead) { Died(); }

        // Check if on ground or wall
        CheckTouchingTerrain();

        // dash resets and wall climb when player touches ground
        if (_playerState.onGround) 
        {
            _playerState.canDash = true;
            _playerState.canWallClimb = true;
            if (_walkParticles.isPaused || _walkParticles.isStopped) _walkParticles.Play();
        } else
        {
            _walkParticles.Stop();
        }

        // turn gravity back on when done sling shot
        if (!_playerState.slingShotActive) 
        {
            _rigidBody.gravityScale = _gravityScale;
        }

        _horizontalAxis = _controls.Player.Movement.ReadValue<Vector2>().x;
        _verticalAxis = _controls.Player.Movement.ReadValue<Vector2>().y;

        bool jumpKeyPressed = _controls.Player.Jump.triggered;
        bool dashKeyPressed = _controls.Player.Dash.triggered;
        bool slingShotPressed = _controls.Player.Orb.ReadValue<float>() > 0.1f ? true : false;

        if (slingShotPressed)
        {
            _playerState.orbPulled = true;
        }
        else if (_playerState.slingShotReleased)
        {
            _playerState.orbPulled = false;
        }

        if (slingShotPressed && _playerState.isOrbActive)
        {
            _playerState.currentMovementState = MovementState.SlingShotActive;
        }
        else if (_playerState.slingShotReleased)
        {
            _playerState.slingShotActive = false;
            _playerState.currentMovementState = MovementState.Default;
        }
        else if (jumpKeyPressed)
        {
            _playerState.slingShotActive = false;
            _playerState.currentMovementState = MovementState.JumpActive;
        }
        else if (dashKeyPressed && _playerState.canDash)
        {
            _playerState.slingShotActive = false;
            _playerState.currentMovementState = MovementState.DashActive;
            _playerState.canDash = false;
        }

        if (_playerState.isOrbActive)
        {
            Debug.DrawLine(_activeOrbPosition, _transform.position, Color.red);
        }

        UpdateAnimationState(_playerState.onGround, _playerState.onWall, _playerState.orbPulled, _playerState.isOrbActive);

        _playerState.slingShotReleased = false;
    }

    /**
    * Determine which physics movement operations to execture based on current movement states
    */
    private void FixedUpdate() {
        switch (_playerState.currentMovementState) {
            case MovementState.DashActive: Horizontal_Dash(); break;
            case MovementState.JumpActive: Vertical_Jump(); break;
            case MovementState.SlingShotActive: SlingShot(); break;
            case MovementState.Default: 
            {
                Horizontal_Move();
                Vertical_Move(); 
                break;
            } 
        }
    }

    #region Movement Methods

    /**
    * Moves the player object left or right
    */
    private void Horizontal_Move() {

        UpdateDirectionFacing();

        var forceVector = _horizontalAxis * _speed;

        _rigidBody.AddForce(new Vector2(forceVector, 0));
    }

    /**
    * Gives the player quick burst of horizontal movement in the direction the player is facing over a fixed time interval
    */
    private void Horizontal_Dash() {
        _rigidBody.velocity = new Vector2(0, 0);
        _rigidBody.AddForce(new Vector2(((float)_playerState.currentDirectionFacing) * _jumpForce, _verticalComponentDashForce));
        _dashParticles.Play();
        _playerState.currentMovementState = MovementState.Default;
    }
    
    /**
    * Accelerate the player vertically up
    */
    private void Vertical_Jump() {

        if (_playerState.onWall) {
            Vertical_WallJump();
            return;
        }

        // reset jumps when on the ground
        if (_playerState.onGround) {
            _playerState.jumpCounter = 0;
        }

        if (_playerState.jumpCounter < _maxJumps) {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
            _playerState.jumpCounter++;
        }

        _playerState.currentMovementState = MovementState.Default;
    }

    /**
    * Accelerate the player away from the wall in an upward direction
    */
    private void Vertical_WallJump() {
        _rigidBody.velocity = new Vector2(0, 0);
        _rigidBody.AddForce(new Vector2(((float) _playerState.currentDirectionFacing) * _jumpForce, _jumpForce));
        _playerState.jumpCounter = 1;
        _playerState.currentMovementState = MovementState.Default;
    }

    /**
    * Determine which vertical movement should be executed
    */
    private void Vertical_Move() {
        if (_verticalAxis > 0 && _playerState.onWall) {
            Vertical_WallClimb();
        } else if (_verticalAxis < 0 && !_playerState.onWall && !_playerState.onGround) {
            Vertical_AccelerateDown();
        }
    }

    /**
    * Makes player fall faster
    */
    private void Vertical_AccelerateDown() {
        _rigidBody.AddForce(new Vector2(0, -_downForce));
    }

    /**
    * Move player upward along a wall
    */
    private void Vertical_WallClimb() {
        if (_playerState.canWallClimb) {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
            _playerState.canWallClimb = false;
        }
    }

    private void SlingShot() {
        if (_playerState.isOrbActive) {
            if (!_playerState.slingShotActive) {
                _playerState.jumpCounter = 1;
                _playerState.slingShotActive = true;
                _rigidBody.gravityScale = 0;
            }

            Vector2 direction = (_activeOrbPosition - transform.position).normalized;
            
            _rigidBody.AddForce(direction * _orbForceMultiplier);
        }
    }

    #endregion

    /**
    * Checks to see if player is touching the ground or walls
    */
    private void CheckTouchingTerrain() 
    {
        // max distance from the collider in which to register a collision
        float touchingGroundBuffer = 0.1f;

        // creates small area just underneath the Player Collider and checks if any object on the terrain layer is inside this area
        _playerState.onGround = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, _terrainLayer.value);     

        // onGround take priority over onWall;
        if (_playerState.onGround) {
            _playerState.onLeftWall = false;
            _playerState.onRightWall = false;
            return;
        }

        // creates small area just to the left and right of the Player Collider and checks if any object on the terrain layer is inside this area
        _playerState.onLeftWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.left, touchingGroundBuffer, _terrainLayer.value);
        _playerState.onRightWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.right, touchingGroundBuffer, _terrainLayer.value);

        UpdateDirectionFacing();
    }

    /**
    * correctly rotates player object depending on which way it is moving
    */
    private void UpdateDirectionFacing() {
        if (_playerState.onWall) {
            // if on wall face away from the wall
            if (_playerState.onLeftWall) {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                _playerState.currentDirectionFacing = DirectionFacing.Right;
            } else {
                transform.rotation = new Quaternion(0, 180, 0, 0);
                _playerState.currentDirectionFacing = DirectionFacing.Left;
            }
        } else if (_horizontalAxis < 0 && _playerState.currentDirectionFacing == DirectionFacing.Right) {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            _playerState.currentDirectionFacing = DirectionFacing.Left;
        } else if (_horizontalAxis > 0 && _playerState.currentDirectionFacing == DirectionFacing.Left) {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            _playerState.currentDirectionFacing = DirectionFacing.Right;
        }
    }

    public void SetActiveOrb(bool isActive, Vector3 activeOrbPos) {
        var distanceToCurrentOrb = Vector2.Distance(_activeOrbPosition, _transform.position);
        var distanceToNewOrb = Vector2.Distance(activeOrbPos, _transform.position);
        if (_activeOrbPosition.Equals(Vector3.zero) || distanceToNewOrb.CompareTo(distanceToCurrentOrb) <= 0)
        {
            _activeOrbPosition = activeOrbPos;
        }
        _playerState.isOrbActive = isActive;
    }

    private void Died()
    {
        // probably wait until death animation is finished before setting _transform.position
        _playerState.currentMovementState = MovementState.Default;
        _playerState.isDead = true;
        _rigidBody.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        DeathAnimation();
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(1);
        _transform.position = _respawnPosition;
        _rigidBody.constraints ^= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        _playerState.isDead = false;
    }

    #region GameObject Events

    void OnCollisionEnter2D(Collision2D col)
    {
        if ((1 << col.gameObject.layer) == _harmLayer.value && !_playerState.isDead)
        {
            Died();
            Debug.Log("Player died");
        }
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDsiable()
    {
        _controls.Disable();
    }

    private void OnDestroy()
    {
        UpdateAnimationState -= _animation.SetAnimationState;
        DeathAnimation -= _animation.SetDeathState;
    }

    #endregion
}