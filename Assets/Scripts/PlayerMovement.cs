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
    private int jumpCounter = 0;
    private int maxJumps = 2;
    private bool jumpPressed = false;
    private bool onGround = false;

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
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        
        if (Input.GetButtonDown("Jump") && !jumpPressed) {
            jumpActive = true;
            jumpPressed = true;
        } else if (Input.GetButtonUp("Jump")) {
            jumpPressed = false;
        }
    }

    void FixedUpdate() {
        MoveHorizontal(horizontalAxis * speed * Time.deltaTime);
        if (verticalAxis < 0) AccelerateDown();
        if (jumpActive) Jump();
        checkVerticalVeclocity();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if ((1 << col.collider.gameObject.layer) == terrainLayer.value) {
            Debug.Log("Terrain collision");
            onGround = true;
        }
    }

    #region Movement Methods

    //to implement double jump will need to check - can't use on collision./
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
    
    private void MoveHorizontal(float s) {
        //will need to change facing direction
        rigidbody.velocity = new Vector2(s, rigidbody.velocity.y);
    }

    private void AccelerateDown() {
        Debug.Log("Down");
        rigidbody.AddForce(new Vector2(0, -downForce));
    }

    private void checkVerticalVeclocity() {
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
