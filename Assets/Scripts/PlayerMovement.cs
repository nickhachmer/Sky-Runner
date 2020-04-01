using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region PlayerMovement Variables
    // player 
    public BoxCollider2D boxCollider;
    public new Rigidbody2D rigidbody;
    public LayerMask terrainLayer;

    // public player values set in editor
    public int speed;
    public int jumpForce; 
    public int downForce;
    public int maxVerticalVelocity;


    //private player values
    private float horizontalAxis = 0;
    private float verticalAxis = 0;
    private bool jumpActive = false;
    private int jumpCounter = 2;
    private bool onGround = false;
    private bool canAirJump = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {   

        //boxCollider = GetComponent<BoxCollider2D>();
        //rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        // activate jump
        jumpActive = Input.GetButton("Jump");
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
        Debug.Log("jump");
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        rigidbody.AddForce(new Vector2(0, jumpForce));
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
    #endregion
}
