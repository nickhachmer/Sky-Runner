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
    }

    public void SetDeathState()
    {
        _animator.SetInteger("Surface", (int) SurfaceMapping.Death);
    }
}
