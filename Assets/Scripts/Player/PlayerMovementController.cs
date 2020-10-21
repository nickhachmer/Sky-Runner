using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Player Enums
    public enum MovementState : short
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
    [SerializeField] private new Rigidbody2D _rigidbody = default;
    [SerializeField] private int _speed = default;
    [SerializeField] private int _jumpForce = default; 
    [SerializeField] private int _downForce = default;

    // private player values
    private LayerMask _terrainLayer;
    private Vector3 _activeOrbPosition = new Vector3(0, 0, 0);
    private bool _isOrbActive = false;

    private float horizontalAxis = 0;
    private float verticalAxis = 0;

    private int maxJumps = 2;
    private int jumpCounter = 0;
    
    private bool canDash = false;
    private bool canWallClimb = false;
    private bool slingShotActive = false;
    
    private bool onGround = false;
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private bool onWall {
        get {
            return onLeftWall || onRightWall;
        }
    }
  
    private float axisBuffer = 0.2f;
    private float gravityScale;
    private DirectionFacing currentDirectionFacing;
    private MovementState currentMovementState;
    #endregion


    void Awake() {
        _terrainLayer = LayerMask.NameToLayer("Terrain");
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        gravityScale = _rigidbody.gravityScale;
    }
    
    void Start()
    {
        currentDirectionFacing = DirectionFacing.Right;
        currentMovementState = MovementState.Default;
    }

    /** 
    * Capture inputs and determine current movement state
    */
    void Update()
    {
        // Check if on ground or wall
        CheckTouchingTerrain();

        // dash resets and wall climb when player touches ground
        if (onGround) 
        {
            canDash = true;
            canWallClimb = true;
        }

        // turn gravity back on when done sling shot
        if (!slingShotActive) 
        {
            _rigidbody.gravityScale = gravityScale;
        }

        horizontalAxis = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > axisBuffer? Input.GetAxisRaw("Horizontal") : 0;
        verticalAxis = Mathf.Abs(Input.GetAxisRaw("Vertical")) > axisBuffer? Input.GetAxisRaw("Vertical") : 0;

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
            currentMovementState = MovementState.SlingShotActive;
        }
        else if (slingShotReleased)
        {
            slingShotActive = false;
            currentMovementState = MovementState.Default;
        }
        else if (jumpKeyPressed) 
        {
            slingShotActive = false;
            currentMovementState = MovementState.JumpActive;
        } 
        else if (dashKeyPressed && canDash) 
        {
            slingShotActive = false;
            currentMovementState = MovementState.DashActive;
            canDash = false;
        }
    }

    /**
    * Determine which physics movement operations to execture based on current movement states
    */
    void FixedUpdate() {
        switch (currentMovementState) {
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

        _rigidbody.AddForce(new Vector2(horizontalAxis * _speed, 0));
    }

    /**
    * Gives the player quick burst of horizontal movement in the direction the player is facing over a fixed time interval
    */
    private void Horizontal_Dash() {
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.AddForce(new Vector2(((float) currentDirectionFacing) * _jumpForce, 10));
        currentMovementState = MovementState.Default;
    }
    
    /**
    * Accelerate the player vertically up
    */
    private void Vertical_Jump() {

        if (onWall) {
            Vertical_WallJump();
            return;
        }

        // reset jumps when on the ground
        if (onGround) {
            jumpCounter = 0;
        }

        if (jumpCounter < maxJumps) {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(new Vector2(0, _jumpForce));
            jumpCounter++;
        }

        currentMovementState = MovementState.Default;
    }

    /**
    * Accelerate the player away from the wall in an upward direction
    */
    private void Vertical_WallJump() {
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.AddForce(new Vector2(((float) currentDirectionFacing) * _jumpForce, _jumpForce));
        jumpCounter = 1;
        currentMovementState = MovementState.Default;
    }

    /**
    * Determine which vertical movement should be executed
    */
    private void Vertical_Move() {
        if (verticalAxis > 0 && onWall) {
            Vertical_WallClimb();
        } else if (verticalAxis < 0 && !onWall && !onGround) {
            Vertical_AccelerateDown();
        }
    }

    /**
    * Makes player fall faster
    */
    private void Vertical_AccelerateDown() {
        _rigidbody.AddForce(new Vector2(0, -_downForce));
    }

    /**
    * Move player upward along a wall
    */
    private void Vertical_WallClimb() {
        if (canWallClimb) {
            _rigidbody.AddForce(new Vector2(0, _jumpForce));
            canWallClimb = false;
        }
    }

    private void SlingShot() {
        if (_isOrbActive) {
            if (!slingShotActive) {
                jumpCounter = 0;
                slingShotActive = true;
                _rigidbody.gravityScale = 0;
            }

            Vector2 direction = (_activeOrbPosition - transform.position).normalized;
            
            _rigidbody.AddForce(direction * 150);
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
        onGround = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, layerValue);
        
        // onGround take priority over onWall;
        if (onGround) {
            return;
        }

        // creates small area just to the left and right of the Player Collider and checks if any object on the terrain layer is inside this area
        onLeftWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.left, touchingGroundBuffer, layerValue);
        onRightWall = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.right, touchingGroundBuffer, layerValue);

        UpdateDirectionFacing();

        //Debug.DrawRay(boxCollider.bounds.center, Vector2.down * boxCollider.bounds.size, Color.green);
        //Debug.Log("On ground: " + onGround);
        //Debug.Log("On wall: " + onWall);
    }

    /**
    * correctly rotates player object depending on which way it is moving
    */
    private void UpdateDirectionFacing() {
        if (onWall) {
            // if on wall face away from the wall
            if (onLeftWall) {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                currentDirectionFacing = DirectionFacing.Right;
            } else {
                transform.rotation = new Quaternion(0, 180, 0, 0);
                currentDirectionFacing = DirectionFacing.Left;
            }
        } else if (horizontalAxis < 0 && currentDirectionFacing == DirectionFacing.Right) {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            currentDirectionFacing = DirectionFacing.Left;
        } else if (horizontalAxis > 0 && currentDirectionFacing == DirectionFacing.Left) {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            currentDirectionFacing = DirectionFacing.Right;
        }
    }

    public void SetActiveOrb(bool isActive, Vector3 activeOrbPos) {
        _isOrbActive = isActive;
        _activeOrbPosition = activeOrbPos;
    }
    
}