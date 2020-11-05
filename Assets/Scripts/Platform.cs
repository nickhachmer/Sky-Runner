using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public bool IsPlayerTouching;

    void Start()
    {
        IsPlayerTouching = false;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            IsPlayerTouching = true;
        }
    } 
    
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            IsPlayerTouching = false;
        }
    }

}
