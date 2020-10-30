using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private enum SurfaceMapping : int 
    {
        Death = 0,
        Orb = 1,
        Ground = 2,
        Wall = 3,
        Air = 4
    }

    [SerializeField] private BoxCollider2D _boxCollider = default;
    [SerializeField] private Rigidbody2D _rigidBody = default;
    [SerializeField] private Animator _animator = default;

    public void SetAnimationState(bool onGround, bool onWall, bool fPressed, bool isOrbActive)
    {
        bool orbPulling = fPressed && isOrbActive;
        SurfaceMapping mapping = default;
        if (orbPulling)
        {
            mapping = SurfaceMapping.Orb;
        }
        else if (!orbPulling && onGround)
        {
            mapping = SurfaceMapping.Ground;
        }
        else if (!orbPulling && !onGround && onWall)
        {
            mapping = SurfaceMapping.Wall;
        }
        else if (!orbPulling && !onGround && !onWall)
        {
            mapping = SurfaceMapping.Air;
        }

        var velocity = Mathf.Abs(_rigidBody.velocity.magnitude);
        var angle = Vector3.SignedAngle(Vector3.right, _rigidBody.velocity, Vector3.forward);

        _animator.SetInteger("Surface", (int) mapping);
        _animator.SetFloat("Speed", velocity);
        _animator.SetFloat("VelocityAngle", angle);

        UpdateBoxCollider(mapping, velocity, angle);
    }

    public void SetDeathState()
    {
        _animator.SetInteger("Surface", (int) SurfaceMapping.Death);
    }

    private void UpdateBoxCollider(SurfaceMapping mapping, double velocity, double angle) 
    {
        // AirDash bounds
        // TODO: refactor out these "magic numbers" into consts
        if (mapping == SurfaceMapping.Air && velocity > 10 && (angle < 0 && angle > -45 || angle < -135 && angle > -180))
        {
            _boxCollider.size = new Vector2(0.26f, 0.15f);
            _boxCollider.offset = new Vector2(0.05f, 0.075f);
        }
        else if (mapping == SurfaceMapping.Orb && velocity > 0.01)
        {
            _boxCollider.size = new Vector2(0.19f, 0.19f);
            _boxCollider.offset = new Vector2(0, 0);
        }
        else if (mapping == SurfaceMapping.Wall && velocity > 0.01 && angle > 80 && angle < 100)
        {
            _boxCollider.size = new Vector2(0.19f, 0.35f);
            _boxCollider.offset = new Vector2(0, 0.17f);
        }
        else
        {
            _boxCollider.size = new Vector2(0.19f, 0.3f);
            _boxCollider.offset = new Vector2(0, 0.145f);
        }
    }
}
