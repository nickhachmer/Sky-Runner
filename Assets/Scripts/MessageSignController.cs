using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSignController : MonoBehaviour
{
    [SerializeField] [TextArea(3, 10)] private string _message = default;
    
    public string getMessage()
    {
        return _message;
    }
}
