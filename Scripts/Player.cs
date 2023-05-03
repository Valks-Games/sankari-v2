namespace Sankari.Platformer2D;

public partial class Player : CharacterBody2D
{
    private float maxSpeed = 400;
    private float acceleration = 40;
    private float friction = 20;
    private float gravity = 20;
    private float jumpForce = 100;
    private float jumpLoss = 7.5f;

    private float jumpLossBuildUp;
    private bool holdingJumpKey;
    private readonly Dictionary<string, RayCast2D[]> rayCasts = new();

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Grounded;

        PrepareRaycasts("Floor");
        PrepareRaycasts("Wall Left");
        PrepareRaycasts("Wall Right");
    }

    public override void _PhysicsProcess(double delta)
    {
        var horzDir = Input.GetAxis("move_left", "move_right");
        var vel = Velocity;

        // Horizontal movement
        vel.X += horzDir * acceleration;
        vel.X = Utils.ClampAndDampen(vel.X, friction, maxSpeed);

        // Jump
        if (Input.IsActionJustPressed("jump") && AreRaycastsTouching("Floor"))
        {
            holdingJumpKey = true;
            jumpLossBuildUp = 0;
            vel.Y -= jumpForce;
        }

        if (Input.IsActionPressed("jump") && holdingJumpKey)
        {
            jumpLossBuildUp += jumpLoss;
            vel.Y -= Mathf.Max(0, jumpForce - jumpLossBuildUp);
        }

        if (Input.IsActionJustReleased("jump"))
        {
            holdingJumpKey = false;
        }

        // Gravity
        vel.Y += gravity;

        Velocity = vel;
        MoveAndSlide();
    }

    private void PrepareRaycasts(string type)
    {
        var raycastsFloor = GetNode($"Raycasts/{type}").GetChildren<RayCast2D>();

        rayCasts[type] = new RayCast2D[raycastsFloor.Length];

        for (int i = 0; i < raycastsFloor.Length; i++)
        {
            rayCasts[type][i] = raycastsFloor[i];
            rayCasts[type][i].AddException(this);
        }
    }

    private bool AreRaycastsTouching(string type)
    {
        foreach (var raycast in rayCasts[type])
            if (raycast.IsColliding())
                return true;

        return false;
    }
}
