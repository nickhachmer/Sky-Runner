using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Components
    public BoxCollider2D boxCollider;
    public new Rigidbody2D rigidbody;

    // Player Values
    public int speed = 10;
    public int jumpForce = 300;
    

    // Start is called before the first frame update
    void Start()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        //rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            rigidbody.AddForce(new Vector2(0, jumpForce));
        }


    }
}
