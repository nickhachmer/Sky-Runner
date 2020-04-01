using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // player components
    public BoxCollider2D boxCollider;
    public new Rigidbody2D rigidbody;

    // public player values set in editor
    public int speed;
    public int jumpForce;
    public LayerMask collisionLayer;
    
    //private player values
    private float horizontalAxis = 0;
    private bool jump = false;

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
        jump = Input.GetButton("Jump");

        //if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.S)) Down();
    }

    void FixedUpdate() {
        MoveHorizontal(horizontalAxis * speed * Time.deltaTime);
        if (jump) Jump();
    }

    private void Jump() {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        rigidbody.AddForce(new Vector2(0, jumpForce));
    }
    
    private void MoveHorizontal(float s) {
        //will need to change facing direction
        rigidbody.velocity = new Vector2(s, rigidbody.velocity.y);
    }

    private void Down() {

    }
}
