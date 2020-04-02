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
    public int dashForce;
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

    #endregion

    // used for intialization
    void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {

        // Horizontal Movement
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashPressed) {
            startTime = Time.fixedTime;
            dashActive = true;
            dashPressed = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift)) {
            dashPressed = false;
        }


        // Vertical Movement
        verticalAxis = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump") && !jumpPressed) {
            jumpActive = true;
            jumpPressed = true;
        } else if (Input.GetButtonUp("Jump")) {
            jumpPressed = false;
        }
    }

    void FixedUpdate() {
        // Horizontal Movement
        if (dashActive) {
            HorizontalDash();
        } else {
            MoveHorizontal();
        }

        // Vertical Movement
        if (verticalAxis < 0) AccelerateDown();
        if (jumpActive) Jump();
        limitVericalVeclocity();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if ((1 << col.collider.gameObject.layer) == terrainLayer.value) {
            Debug.Log("Terrain collision");
            onGround = true;
        }
    }

    #region Movement Methods

    private void MoveHorizontal() {        
        rigidbody.velocity = new Vector2(horizontalAxis * speed * Time.deltaTime, rigidbody.velocity.y);
    }

    private void HorizontalDash() {
        float timeDifference = Time.fixedTime - startTime;
        rigidbody.velocity = new Vector2(-70 * timeDifference + 30, 0);
        if (timeDifference >= 0.3f) {
            dashActive = false;
        }
    }
    
    //to implement double jump will need to check - can't use on collision - may cause problems with wall climb
    private void Jump() {
        if (onGround) jumpCounter = 0; 
        if (onGround || jumpCounter < maxJumps) {
            jumpCounter++;
            onGround = false;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            rigidbody.AddForce(new Vector2(0, jumpForce));
        }
        jumpActive = false;
    }

    private void AccelerateDown() {
        Debug.Log("Down");
        rigidbody.AddForce(new Vector2(0, -downForce));
    }

    private void limitVericalVeclocity() {
        if (rigidbody.velocity.y >= maxVerticalVelocity) {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, maxVerticalVelocity);
        } else if (rigidbody.velocity.y <= -maxVerticalVelocity) {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -maxVerticalVelocity);
        }
    }

    // private bool CheckTouchingGround() 
    // {
    //     float touchingGroundBuffer = 1.0f;
    //     bool onGround = false;
    //     onGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, touchingGroundBuffer, terrainLayer.value);
    //     Debug.DrawRay(boxCollider.bounds.center, Vector2.down * boxCollider.bounds.size, Color.green);
    //     Debug.Log(onGround);
    //     return onGround;
    // }

    #endregion
}
