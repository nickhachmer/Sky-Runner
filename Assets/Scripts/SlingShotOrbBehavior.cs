﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotOrbBehavior : MonoBehaviour
{
    [SerializeField] private PhysicsDatabase _physicsDatabase = default;
    [SerializeField] private string _terrainLayerName = default;
    [SerializeField] private PlayerMovementController _playerMovementController = default;

    private Transform _playerTransform;
    private LayerMask _terrainLayer;
    private short _orbRange = default;
    private bool _isActive = false;

    void Awake()
    {
        _terrainLayer = LayerMask.NameToLayer(_terrainLayerName);
        _orbRange = _physicsDatabase.OrbRange;
        _playerTransform = _playerMovementController.Transform;
    }

    void Update()
    {
        // requires refactor only need to send the position once per active - this does it every frame

        bool isTerrainInBetween = Physics2D.Linecast(transform.position, _playerTransform.position, 1 << _terrainLayer.value);

        if (!isTerrainInBetween && Vector2.Distance(transform.position, _playerTransform.position) < _orbRange) {
            _isActive = true;
            _playerMovementController.SetActiveOrb(true, transform.position);
        } else {
            _isActive = false;
            _playerMovementController.SetActiveOrb(false, new Vector3(0, 0, 0));
        }


    }
}
