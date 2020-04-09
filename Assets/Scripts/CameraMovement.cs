using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    public Transform playerTransform;
    
    private Camera cameraComponent;
    private short screenBufferY = 7;
    private short screenBufferX = 10;
    
    void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {

        // Keeps player sprite within screen, but does not directly follow
        float x = transform.position.x;
        float y = transform.position.y;
        float diffX = playerTransform.position.x - x;
        float diffY = playerTransform.position.y - y;

        if (diffX > screenBufferX) {
            x = playerTransform.position.x - screenBufferX;
        } else if (diffX < -screenBufferX) {
            x = playerTransform.position.x + screenBufferX;
        }

        if (diffY > screenBufferY) {
            y = playerTransform.position.y - screenBufferY;
        } else if (diffY < -screenBufferY) {
            y = playerTransform.position.y + screenBufferY;
        }


        transform.position = new Vector3(x, y, -10);
    }



}
