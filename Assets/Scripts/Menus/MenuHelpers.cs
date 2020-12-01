using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHelpers
{
    public static void Transition(GameObject from, GameObject to)
    {
        from.SetActive(false);
        to.SetActive(true);
    }
}
