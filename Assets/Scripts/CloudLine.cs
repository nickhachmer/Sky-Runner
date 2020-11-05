using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudLine : MonoBehaviour
{

    [SerializeField] private LineRenderer _lineRenderer = default;
    [SerializeField] private Transform _playerTransform = default;
    [SerializeField] private Platform _platformScript = default;
    [SerializeField] private float _yPosition = default;
    [SerializeField] private float _xPosition = default;

    void Update()
    {
        if (_platformScript.IsPlayerTouching)
        {
            _lineRenderer.SetPosition(1, new Vector3(_playerTransform.position.x - 0.5f, _yPosition, 0));
            _lineRenderer.SetPosition(2, new Vector3(_playerTransform.position.x - 0.3f, _yPosition - 0.2f, 0));
            _lineRenderer.SetPosition(3, new Vector3(_playerTransform.position.x + 0.3f, _yPosition - 0.2f, 0));
            _lineRenderer.SetPosition(4, new Vector3(_playerTransform.position.x + 0.5f, _yPosition, 0));
        } 
        else
        {
            for (int i = 1; i < 5; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(_xPosition, _yPosition, 0));
            }
        }
    }

}
