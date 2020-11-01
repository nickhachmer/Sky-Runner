using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Player Enums
    private enum MovementState : short
    {
        Default = 0,
        JumpActive = 1,
        DashActive = 2,
        SlingShotActive = 3
    }

    private enum DirectionFacing: int
    {
        Right = 1,
        Left = -1
    }

    #endregion

    #region PlayerMovement Variables

    // public player values set in editor
    [SerializeField] private BoxCollider2D _boxCollider = default;
    [SerializeField] private Rigidbody2D _rigidBody = default;
    [SerializeField] private Transform _transform = default;
    [SerializeField] private PlayerAnimationController _animation = default;
    [SerializeField] private PhysicsDatabase _physicsDatabase = default;
    [SerializeField] private LayerMask _terrainLayer = default;
    [SerializeField] private LayerMask _harmLayer = default;
    [SerializeField] private ParticleSystem _dashParticles = default;
    [SerializeField] private ParticleSystem _walkParticles = default;
    [SerializeField] private int _maxJumps = default;
    [SerializeField] private int _yPositionLimit = default;

    // public properties
    public Transform Transform => _transform;

    // private player values
    private int _speed = default; 
    private int _jumpForce = default; 
    private int _downForce = default;
    private int _verticalComponentDashForce = default;
    private float _gravityScale = default; 

    private int _orbForceMultiplier = default;

    private Vector3 _activeOrbPosition = new Vector3(0, 0, 0);
    private bool _isOrbActive = false;

    private float _horizontalAxis = 0;
    private float _verticalAxis = 0;
    private float _axisBuffer = 0.2f;

    private int _jumpCounter = 0;
    
    private bool _canDash = false;
    private bool _canWallClimb = false;
    private bool _slingShotActive = false;
    
    private bool _onGround = false;
    private bool _onLeftWall = false;
    private bool _onRightWall = false;
    private bool _onWall
    {
        get
        {
            return _onLeftWall || _onRightWall;
        }
    }
    private bool _fPressed = false;
  
    private DirectionFacing _currentDirectionFacing = default;
    private MovementState _currentMovementState = default;
    
    private Vector3 _startPosition = default;
    private bool _isDead = false;
    #endregion

    #region Player Actions

    private Action<bool, bool, bool, bool> UpdateAnimationState = default;
    private Action DeathAnimation = default;

    #endregion

    private void Awake() {
        _speed = _physicsDatabase.PlayerSpeed;
        _jumpForce = _physicsDatabase.PlayerJumpForce;
        _downForce = _physicsDatabase.PlayerDownForce;
        _verticalComponentDashForce = _physicsDatabase.PlayerVerticalComponentDashForce;
        _gravityScale = _physicsDatabase.GravityScale;

        _rigidBody.gravityScale = _gravityScale;
        _rigidBody.angularDrag = _physicsDatabase.PlayerAngularDrag;
        _rigidBody.drag = _physicsDatabase.PlayerLinearDrag;
        _rigidBody.mass = _physicsDatabase.PlayerMass;

        _orbForceMultiplier = _physicsDatabase.OrbForceMultiplier;

        _startPosition = Vector3.zero;

        UpdateAnimationState += _animation.SetAnimationState;
        DeathAnimation += _animation.SetDeathState;
    }
    
    private void Start()
    {
        _currentDirectionFacing = DirectionFacing.Right;
        _currentMovementState = MovementState.Default;
    }

    /** 
    * Capture inputs and determine current movement state
    */
    private void Update()
    {
        if (_isDead) {
            _rigidBody.velocity = Vector2.zero;
            return;
        }
 
        if (_transform.position.y < _yPositionLimit) { Died(); }

        // Check if on ground or wall
        CheckTouchingTerrain();

        // dash resets and wall climb when player touches ground
        if (_onGround) 
        {
            _canDash = true;
            _canWallClimb = true;
            if (_walkParticles.isPaused || _walkParticles.isStopped) _walkParticles.Play();
        } else
        {
            _walkParticles.Stop();
        }

        // turn gravity back on when done sling shot
        if (!_slingShotActive) 
        {
            _rigidBody.gravityScale = _gravityScale;
        }

        _horizontalAxis = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > _axisBuffer? Input.GetAxisRaw("Horizontal") : 0;
        _verticalAxis = Mathf.Abs(Input.GetAxisRaw("Vertical")) > _axisBuffer? Input.GetAxisRaw("Vertical") : 0;

        bool jumpKeyPressed = Input.GetButtonDown("Jump");
        bool dashKeyPressed = Input.GetKeyDown(KeyCode.LeftShift);
        bool slingShotPressed = Input.GetKeyDown(KeyCode.F);
        bool slingShotReleased = Input.GetKeyUp(KeyCode.F);
        bool pauseButtonPressed = Input.GetKeyDown(KeyCode.Escape);

        if (slingShotPressed)
        {
            _fPressed = true;
        }
        else if (slingShotReleased)
        {
            _fPressed = false;
        }

        if (slingShotPressed && _isOrbActive) 
        {
            _currentMovementState = MovementState.SlingShotActive;
        }
        else if (slingShotReleased)
        {
            _slingShotActive = false;
            _currentMovementState = MovementState.Default;
        }
        else if (jumpKeyPressed) 
        {
            _slingShotActive = false;
            _currentMovementState = MovementState.JumpActive;
        } 
        else if (dashKeyPressed && _canDash) 
        {
            _slingShotActive = false;
            _currentMovementState = MovementState.DashActive;
            _canDash = false;
        }

        if (_isOrbActive)
        {
            Debug.DrawLine(_activeOrbPosition, _transform.position, Color.red);
        }

        UpdateAnimationState(_onGround, _onWall, _fPressed, _isOrbActive);
    }

    /**
    * Determine which physics movement operations to execture based on current movement states
    */
    private void FixedUpdate() {
        switch (_currentMovementState) {
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

    private void OnDestroy()
    {
        UpdateAnimationState -= _animation.SetAnimationState;
        DeathAnimation -= _animation.SetDeathState;
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
        _rigidBody.AddForce(new Vector2(((float) _currentDirectionFacing) * _jumpForce, _verticalComponentDashForce));
        _dashParticles.Play();
        _currentMovementState = MovementState.Default;
    }
    
    /**
    * Accelerate the player vertically up
    */
    private void Vertical_Jump() {

        if (_onWall) {
            Vertical_WallJump();
            return;
        }

        // reset jumps when on the ground
        if (_onGround) {
            _jumpCounter = 0;
        }

        if (_jumpCounter < _maxJumps) {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
            _jumpCounter++;
        }

        _currentMovementState = MovementState.Default;
    }

    /**
    * Accelerate the player away from the wall in an upward direction
    */
    private void Vertical_WallJump() {
        _rigidBody.velocity = new Vector2(0, 0);
        _rigidBody.AddForce(new Vector2(((float) _currentDirectionFacing) * _jumpForce, _jumpForce));
        _jumpCounter = 1;
        _currentMovementState = MovementState.Default;
    }

    /**
    * Determine which vertical movement should be executed
    */
    private void Vertical_Move() {
        if (_verticalAxis > 0 && _onWall) {
            Vertical_WallClimb();
        } else if (_verticalAxis < 0 && !_onWall && !_onGround) {
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
        if (_canWallClimb) {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
            _canWallClimb = false;
        }
    }

    private void SlingShot() {
        if (_isOrbActive) {
            if (!_slingShotActive) {
                _jumpCounter = 1;
                _slingShotActive = true;
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
        _onGround = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, _terrainLayer.value);     

        // onGround take priority over onWall;
        if (_onGround) {
            _onLeftWall = false;
            _onRightWall = false;
            return;
        }

        // creates small area just to the left and right of the Player Collider and checks if any object on the terrain layer is inside this area
        _onLeftWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.left, touchingGroundBuffer, _terrainLayer.value);
        _onRightWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.right, touchingGroundBuffer, _terrainLayer.value);

        UpdateDirectionFacing();

        //Debug.DrawRay(_boxCollider.bounds.center, Vector2.down * _boxCollider.bounds.size, Color.green);
        //Debug.Log("On ground: " + _onGround);
        //Debug.Log("On wall: " + _onWall);
    }

    /**
    * correctly rotates player object depending on which way it is moving
    */
    private void UpdateDirectionFacing() {
        if (_onWall) {
            // if on wall face away from the wall
            if (_onLeftWall) {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                _currentDirectionFacing = DirectionFacing.Right;
            } else {
                transform.rotation = new Quaternion(0, 180, 0, 0);
                _currentDirectionFacing = DirectionFacing.Left;
            }
        } else if (_horizontalAxis < 0 && _currentDirectionFacing == DirectionFacing.Right) {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            _currentDirectionFacing = DirectionFacing.Left;
        } else if (_horizontalAxis > 0 && _currentDirectionFacing == DirectionFacing.Left) {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            _currentDirectionFacing = DirectionFacing.Right;
        }
    }

    public void SetActiveOrb(bool isActive, Vector3 activeOrbPos) {
        var distanceToCurrentOrb = Vector2.Distance(_activeOrbPosition, _transform.position);
        var distanceToNewOrb = Vector2.Distance(activeOrbPos, _transform.position);
        if (_activeOrbPosition.Equals(Vector3.zero) || distanceToNewOrb.CompareTo(distanceToCurrentOrb) <= 0)
        {
            _activeOrbPosition = activeOrbPos;
        }
        _isOrbActive = isActive;
    }

    private void Died()
    {
        // probably wait until death animation is finished before setting _transform.position
        _currentMovementState = MovementState.Default;
        _isDead = true;
        DeathAnimation();
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(1);
        _transform.position = _startPosition;
        _isDead = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if ((1 << col.gameObject.layer) == _harmLayer.value)
        {
            Died();
            Debug.Log("Player died");
        }
    }
}