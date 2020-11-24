namespace Assets.Scripts.Player
{
    struct PlayerState
    {
        public bool canDash;
        public bool canWallClimb;
        public bool slingShotActive;
        public bool onGround;
        public bool onLeftWall;
        public bool onRightWall;
        public bool orbPulled;
        public bool isDead;
        public bool slingShotReleased;
        public bool isOrbActive;
        public int jumpCounter;
        public bool onWall
        {
            get
            {
                return onLeftWall || onRightWall;
            }
        }

        public DirectionFacing currentDirectionFacing;
        public MovementState currentMovementState;
    };
}