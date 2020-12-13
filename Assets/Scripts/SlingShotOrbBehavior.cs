using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotOrbBehavior : MonoBehaviour
{
    [SerializeField] private PhysicsDatabase _physicsDatabase = default;
    [SerializeField] private Transform _orbRangeIndicator = default;
    [SerializeField] private short _orbRangeOverride = 0;
    private PlayerController _playerController = default;

    private Transform _playerTransform;
    private short _orbRange = default;
    private bool _isActive = false;

    void Awake()
    {
        if (_orbRangeOverride != 0)
        {
            _orbRange = _orbRangeOverride;
        }
        else
        {
            _orbRange = _physicsDatabase.OrbRange;
        }
        
        _orbRangeIndicator.localScale = new Vector3(4 * _orbRange, 4 * _orbRange, 1);

        _playerController = GameManager.Instance.PlayerController;
        _playerTransform = _playerController.Transform;

    }

    void Update()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) < _orbRange)
        {
            _isActive = true;
            _playerController.SetActiveOrb(true, transform.position);
        }
        else if (_isActive)
        {
            _isActive = false;
            _playerController.SetActiveOrb(false, Vector3.zero);
        }

        if (_isActive)
        {
            // makes the orb range indicator change colors
            Color lightBlue = new Color(0.1f, 0.67f, 1f, 0.4f);
            Color darkBlue = new Color(0.4f, 0.4f, 1f, 0.4f);
            _orbRangeIndicator.GetComponent<SpriteRenderer>().color = Color.Lerp(lightBlue, darkBlue, Mathf.PingPong(Time.time, 1));
        }

    }
}
