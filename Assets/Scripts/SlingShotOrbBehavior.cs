using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotOrbBehavior : MonoBehaviour
{
    public bool isActive = false;

    private short orbRange = 10;
    private Transform playerTransform;
    private PlayerMovementController playerMovementScript;
    private LayerMask terrainLayer;

    void Awake()
    {
        terrainLayer = LayerMask.NameToLayer("Terrain");
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        
        // requires refactor only need to send the position once per active - this does it every frame

        bool isTerrainInBetween = Physics2D.Linecast(transform.position, playerTransform.position, 1 << terrainLayer.value);

        if (!isTerrainInBetween && Vector2.Distance(transform.position, playerTransform.position) < orbRange) {
            isActive = true;
            playerMovementScript.SetActiveOrb(true, transform.position);
        } else {
            isActive = false;
            playerMovementScript.SetActiveOrb(false, new Vector3(0, 0, 0));
        }


    }
}
