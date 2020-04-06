using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region PlayerMovement Variables
    // playe
    public BoxCollider2D boxCollider;
    public new Rigidbody2D rigidbody;
    public LayerMask terrainLayer;

    // public player values set in editor
    public int speed;
    public int jumpForce; 
    public int downForce;
    public int maxVerticalVelocity;


    // private player values
    private float horizontalAxis = 0;
    private float verticalAxis = 0;
    private bool jumpActive = false;
    private bool dashActive = false;
    private bool dashPressed = false;
    private int jumpCounter = 0;
    private int maxJumps = 2;
    private bool jumpPressed = false;
    private bool onGround = false;
    private float startTime = 0;
    private bool canDash = false;
    private bool canWallClimb = false;
    private bool onWall {
        get {
            return onLeftWall || onRightWall;
        }
    }
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private bool wallJumpActive = false;
    private float axisBuffer = 0.2f;
    private enum DirectionFacing: int {
        Right = 1,
        Left = -1
    }
    private DirectionFacing currentDirectionFacing = DirectionFacing.Right;

    #endregion

    /**
    * Awake is called once during liftime of script instance
    * 
    * Used for intialization
    */ 
    void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    /**
    * Start is called before the first frame update
    */
    void Start()
    {   
        
    }

    /** 
    * Update is called once per frame
    */
    void Update()
    {
        // Check if on ground or wall
        CheckTouchingTerrain();

        #region Horizontal Movement
        
            // used for moving left to right
            horizontalAxis = Input.GetAxisRaw("Horizontal");

            // dash resets when player touches ground or wall
            if (onGround || onWall) {
                canDash = true;
            }

            // actiavte horizontal dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
                startTime = Time.fixedTime;
                dashActive = true;
                canDash = false;
            }    

        #endregion

        #region Vertical Movement
        
            if (onGround) {
                canWallClimb = true;
            }

            // used to climbing up walls and moving faster while falling
            verticalAxis = Input.GetAxisRaw("Vertical");

            // activate vertical jumping
            if (Input.GetButtonDown("Jump") && !jumpPressed) {

                if (onWall) {
                    startTime = Time.fixedTime;
                    wallJumpActive = true;
                } else {
                    jumpActive = true;
                }

                jumpPressed = true;
            } else if (Input.GetButtonUp("Jump")) {
                jumpPressed = false;
            }

        #endregion
    }

    /**
    * FixedUpdate is called once every 0.02 sec
    * 
    * Executes physics movement operations
    */
    void FixedUpdate() {
        
        #region Horizontal Movement
            
            if (dashActive) {
                Horizontal_Dash();
            } else {
                Horizontal_Move();
            }

        #endregion

        #region Vertical Movement

            if (jumpActive) {
                Vertical_Jump();
            } else if (wallJumpActive) {
                Vertical_WallJump();
            }

            if (verticalAxis < -axisBuffer && !limitDownwardVericalVeclocity()) {
                Vertical_AccelerateDown();
            }

            if (verticalAxis > axisBuffer && onWall && canWallClimb) {
                wallClimb();
            }
            

        #endregion
    }

    // void OnCollisionEnter2D(Collision2D col) {
    //     if ((1 << col.collider.gameObject.layer) == terrainLayer.value) {
    //         Debug.Log("Terrain collision");
    //         onGround = true;
    //     }
    // }

    #region Movement Methods

    /**
    * Moves the player object left or right
    */
    private void Horizontal_Move() {

        UpdateDirectionFacing();

        rigidbody.velocity = new Vector2(horizontalAxis * speed * Time.deltaTime, rigidbody.velocity.y);
    }

    /**
    * Gives the player quick burst of horizontal movement in the direction the player is facing over a fixed time interval
    */
    private void Horizontal_Dash() {
        
        // calculate time since dash was started
        float timeDifference = Time.fixedTime - startTime;
        
        // calculate velocity of dash
        float dashVelocity = -90 * timeDifference + 30;

        // horizontal dash
        rigidbody.velocity = new Vector2(dashVelocity, 0);

        // check to see if dash has ended
        if (timeDifference >= 0.3f) {
            dashActive = false;
        }
    }
    
    /**
    * Accelerate the player vertically up
    */
    private void Vertical_Jump() {

        // reset jumps when on the ground
        if (onGround) {
            jumpCounter = 0;
        }

        if (jumpCounter < maxJumps) {
            
            // vertical jump
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            rigidbody.AddForce(new Vector2(0, jumpForce));

            jumpCounter++;
        }

        // vertical jump has ended
        jumpActive = false;
    }

    /**
    * Accelerate the player away from the wall in an upward direction
    */
    private void Vertical_WallJump() {

        // calculate time since dash was started
        float timeDifference = Time.fixedTime - startTime;

        // calculate velocity of dash
        float wallJumpVelocity = -20 * timeDifference + 20;

        // wall jump
        rigidbody.velocity = new Vector2(((float) currentDirectionFacing) * wallJumpVelocity, wallJumpVelocity - 10);
        
        // check to see if wall jump has ended
        if (timeDifference >= 0.2f) {
            Debug.Log("wall jump end");
            wallJumpActive = false;
            jumpCounter = 1;
        }
    }

    private void Vertical_AccelerateDown() {
        Debug.Log("Down");
        rigidbody.AddForce(new Vector2(0, -downForce));
    }

    /**
    * return bool to indicate if currently limiting
    */
    private bool limitDownwardVericalVeclocity() {
        if (rigidbody.velocity.y <= -maxVerticalVelocity) {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -maxVerticalVelocity);
            return true;
        }
        return false;
    }

    private void wallClimb() {
        rigidbody.velocity = new Vector2(0 , speed * Time.deltaTime);
        canWallClimb = false;
    }

    /**
    * Checks to see if player is touching the ground or walls
    */
    private void CheckTouchingTerrain() 
    {
        // max distance from the collider in which to register a collision
        float touchingGroundBuffer = 0.1f;

        // creates small area just underneath the Player Collider and checks if any object on the terrain layer is inside this area
        onGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, terrainLayer.value);
        
        // onGround take priority over onWall;
        if (onGround) {
            return;
        }

        // creates small area just to the left and right of the Player Collider and checks if any object on the terrain layer is inside this area
        onLeftWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, touchingGroundBuffer, terrainLayer.value);
        onRightWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, touchingGroundBuffer, terrainLayer.value);

        UpdateDirectionFacing();

        //Debug.DrawRay(boxCollider.bounds.center, Vector2.down * boxCollider.bounds.size, Color.green);
        //Debug.Log("On ground: " + onGround);
        //Debug.Log("On wall: " + onWall);
    }


    private void UpdateDirectionFacing() {
        if (onWall) {
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


    #endregion
}