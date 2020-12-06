using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    
    [SerializeField] private Transform _playerTransform = default;
    [SerializeField] private Camera _cameraComponent = default;
    [SerializeField] private float _screenShakeAmount = default;
    [SerializeField] private float _screenShakeLength = default;

    private short _screenBufferY = 2;
    private short _screenBufferX = 5;
    private short _orthographicSize = 10;
    private short _zPosition = -10;

    void Awake()
    {
        _cameraComponent.orthographicSize = _orthographicSize;
    }
    void Update()
    {

        // Keeps player sprite within screen, but does not directly follow
        float x = transform.position.x;
        float y = transform.position.y;
        float diffX = _playerTransform.position.x - x;
        float diffY = _playerTransform.position.y - y;

        if (diffX > _screenBufferX)
        {
            x = _playerTransform.position.x - _screenBufferX;
        }
        else if (diffX < -_screenBufferX)
        {
            x = _playerTransform.position.x + _screenBufferX;
        }

        if (diffY > _screenBufferY)
        {
            y = _playerTransform.position.y - _screenBufferY;
        }
        else if (diffY < -_screenBufferY)
        {
            y = _playerTransform.position.y + _screenBufferY;
        }


        transform.position = new Vector3(x, y, _zPosition);
    }

    // screen shake code obtained from https://www.youtube.com/watch?v=Y8nOgEpnnXo
    public void ShakeScreen()
    {
        InvokeRepeating("BeginShake", 0f, 0.1f);
        Invoke("StopShake", _screenShakeLength);
    }

    private void BeginShake()
    {
        if (_screenShakeAmount > 0)
        {
            Vector3 cameraPosition = _cameraComponent.transform.position;

            float offsetX = Random.value * _screenShakeAmount * 2 - _screenShakeAmount;
            float offsetY = Random.value * _screenShakeAmount * 2 - _screenShakeAmount;
            cameraPosition.x += offsetX;
            cameraPosition.y += offsetY;

            _cameraComponent.transform.position = cameraPosition;
        }
    }

    private void StopShake()
    {
        CancelInvoke("BeginShake");
        _cameraComponent.transform.localPosition = Vector3.zero;
    }

}
