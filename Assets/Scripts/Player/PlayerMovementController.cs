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

    private enum DirectionFacing: int {
        Right = 1,
        Left = -1
    }

    #endregion

    #region PlayerMovement Variables

    // public player values set in editor
    [SerializeField] private BoxCollider2D _boxCollider = default;
    [SerializeField] private Rigidbody2D _rigidBody = default;
    [SerializeField] private Transform _transform = default;
    [SerializeField] private PhysicsDatabase _physicsDatabase = default;
    [SerializeField] private string _terrainLayerName = default;
    [SerializeField] private int _maxJumps = 2;

    // public properties
    public Transform Transform => _transform;

    // private player values
    private int _speed = default; 
    private int _jumpForce = default; 
    private int _downForce = default; 
    private float _gravityScale = default; 

    private int _orbForceMultiplier = default;

    private LayerMask _terrainLayer = default;
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
    private bool _onWall {
        get {
            return _onLeftWall || _onRightWall;
        }
    }
  
    private DirectionFacing _currentDirectionFacing = default;
    private MovementState _currentMovementState = default;
    #endregion

    void Awake() {
        _terrainLayer = LayerMask.NameToLayer(_terrainLayerName);
        _speed = _physicsDatabase.PlayerSpeed;
        _jumpForce = _physicsDatabase.PlayerJumpForce;
        _downForce = _physicsDatabase.PlayerDownForce;
        _gravityScale = _physicsDatabase.GravityScale;

        _rigidBody.gravityScale = _gravityScale;
        _rigidBody.angularDrag = _physicsDatabase.PlayerAngularDrag;
        _rigidBody.drag = _physicsDatabase.PlayerLinearDrag;
        _rigidBody.mass = _physicsDatabase.PlayerMass;

        _orbForceMultiplier = _physicsDatabase.OrbForceMultiplier;
    }
    
    void Start()
    {
        _currentDirectionFacing = DirectionFacing.Right;
        _currentMovementState = MovementState.Default;
    }

    /** 
    * Capture inputs and determine current movement state
    */
    void Update()
    {
        // Check if on ground or wall
        CheckTouchingTerrain();

        // dash resets and wall climb when player touches ground
        if (_onGround) 
        {
            _canDash = true;
            _canWallClimb = true;
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

        if (pauseButtonPressed && !GameManager.isGamePaused)
        {
            GameManager.Instance.PauseGame();
        }
        else if (pauseButtonPressed && GameManager.isGamePaused)
        {
            GameManager.Instance.ResumeGame();
        }
        else if (slingShotPressed && _isOrbActive) 
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
    }

    /**
    * Determine which physics movement operations to execture based on current movement states
    */
    void FixedUpdate() {
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

    #region Movement Methods

    /**
    * Moves the player object left or right
    */
    private void Horizontal_Move() {

        UpdateDirectionFacing();

        _rigidBody.AddForce(new Vector2(_horizontalAxis * _speed, 0));
    }

    /**
    * Gives the player quick burst of horizontal movement in the direction the player is facing over a fixed time interval
    */
    private void Horizontal_Dash() {
        _rigidBody.velocity = new Vector2(0, 0);
        _rigidBody.AddForce(new Vector2(((float) _currentDirectionFacing) * _jumpForce, 10));
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
                _jumpCounter = 0;
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
        int layerValue = 1 << _terrainLayer.value;

        // creates small area just underneath the Player Collider and checks if any object on the terrain layer is inside this area
        _onGround = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, layerValue);

        // onGround take priority over onWall;
        if (_onGround) {
            return;
        }

        // creates small area just to the left and right of the Player Collider and checks if any object on the terrain layer is inside this area
        _onLeftWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.left, touchingGroundBuffer, layerValue);
        _onRightWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.right, touchingGroundBuffer, layerValue);

        UpdateDirectionFacing();

        //Debug.DrawRay(boxCollider.bounds.center, Vector2.down * boxCollider.bounds.size, Color.green);
        //Debug.Log("On ground: " + onGround);
        //Debug.Log("On wall: " + onWall);
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
        _isOrbActive = isActive;
        _activeOrbPosition = activeOrbPos;
    }
    
}