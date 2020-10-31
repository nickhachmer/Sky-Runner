using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotOrbBehavior : MonoBehaviour
{
    [SerializeField] private PhysicsDatabase _physicsDatabase = default;
    [SerializeField] private string _terrainLayerName = default;
    [SerializeField] private Transform _orbRangeIndicator = default;
    private PlayerMovementController _playerMovementController = default;

    private Transform _playerTransform;
    private LayerMask _terrainLayer;
    private short _orbRange = default;
    private bool _isActive = false;

    void Awake()
    {
        _terrainLayer = LayerMask.NameToLayer(_terrainLayerName);
        
        _orbRange = _physicsDatabase.OrbRange;
        _orbRangeIndicator.localScale = new Vector3(4 * _orbRange, 4 * _orbRange, 1);

        _playerMovementController = GameManager.Instance.getPlayerMovementController();
        _playerTransform = _playerMovementController.Transform;

    }

    void Update()
    {
        bool isTerrainInBetween = Physics2D.Linecast(transform.position, _playerTransform.position, 1 << _terrainLayer.value);

        if (!isTerrainInBetween && Vector2.Distance(transform.position, _playerTransform.position) < _orbRange)
        {
            _isActive = true;
            _playerMovementController.SetActiveOrb(true, transform.position);
        }
        else if (_isActive)
        {
            _isActive = false;
            _playerMovementController.SetActiveOrb(false, Vector3.zero);
        }

        if (_isActive)
        {
            
            Color lightBlue = new Color(0.1f, 0.67f, 1f, 0.4f);
            Color darkBlue = new Color(0.4f, 0.4f, 1f, 0.4f);
            _orbRangeIndicator.GetComponent<SpriteRenderer>().color = Color.Lerp(lightBlue, darkBlue, Mathf.PingPong(Time.time, 1));
        }

    }
}
