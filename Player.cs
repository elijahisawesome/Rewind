using Godot;
using System;

public partial class Player : CharacterBody3D
{

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public Rid playerRid;
	public int id;
	MultiplayerManager mpm;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public override void _Ready()
    {
		mpm = GetParent<Node>().GetChild<MultiplayerManager>(2);
        playerRid = GetRid();
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	public void takeDamage(playerHitPacket pkt){
		//display some kinda damage indicator
		GD.Print("Shot thru the heart, and youre to blame. Uopi give loooove a bad name, da nua na an aa na na na na");
		GD.Print(pkt.damage);
		Velocity = (Transform.Basis * new Vector3(500,0,500));
		MoveAndSlide();
		//alter health

		//etc
	}
	public void enemyHit(MPlayer target){
		GD.Print(target.port);
		GD.Print(target.hostPort);
		GD.Print(target.id);
		mpm.playerHit(target.id, id);
	}
}