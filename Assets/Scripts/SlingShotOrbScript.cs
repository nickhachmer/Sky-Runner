using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotOrbScript : MonoBehaviour
{
    
    public bool isActive = false;

    private short orbRange = 10;
    private Transform playerTransform;
    private PlayerMovement playerMovementScript;
    private LayerMask terrainLayer;

    void Awake()
    {
        terrainLayer = LayerMask.NameToLayer("Terrain");
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isTerrainInBetween = Physics2D.Linecast(transform.position, playerTransform.position, 1 << terrainLayer.value);

        if (!isTerrainInBetween && Vector2.Distance(transform.position, playerTransform.position) < orbRange) {
            isActive = true;
            playerMovementScript.SetActiveOrb(transform.position);
        } else {
            isActive = false;
        }


    }
}
