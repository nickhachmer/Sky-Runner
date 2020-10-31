// Reference/Source: https://www.youtube.com/watch?v=zit45k6CUMk&t=411s
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject _camera = default;
    [SerializeField] private float _parallaxEffect = default;
    private float _startPosition = default;


    void Start()
    {
        _startPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (_camera.transform.position.x * _parallaxEffect);

        transform.position = new Vector3(_startPosition + distance, transform.position.y, transform.position.z);
    }
}
