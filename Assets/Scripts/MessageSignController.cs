using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSignController : MonoBehaviour
{
    [SerializeField] private string _message = default;
    
    public string getMessage()
    {
        return _message;
    }
}
