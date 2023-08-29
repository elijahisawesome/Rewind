using Godot;
using System;
using System.Diagnostics;


public partial class Player : CharacterBody3D
{

	public const float walkSpeed = 7.0f;
	public const float JumpVelocity = 3.5f;

	public const float maxSpeed = 15f;
	public const float moveSpeed = 5f;
	public const float walkAccel = 10f;
	public const float fallSpeed = 10f;

	public const float stopSpeed = 1f;
	public const float frictionFactor= 3.5f;
	Vector3 wishVel;

	Char animChar = 'I';
	public Rid playerRid;
	public int id;
	MultiplayerManager mpm;
	StateMachine stateMachine;
	bool dashing;
	himbo_base characterModel;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public override void _Ready()
    {
		mpm = GetParent<Node>().GetChild<MultiplayerManager>(2);
		characterModel = GetChild<himbo_base>(5);
        playerRid = GetRid();
		stateMachine = new StateMachine(this);
    }
    public override void _PhysicsProcess(double delta)
	{

		

		// Add the gravity.
		/*
		
		Vector3 velocity = Velocity;
		if (IsOnFloor()){
			walk(delta);	
		}
		else{
			//applyGravity();
			Velocity += new Vector3(0,-fallSpeed*(float)delta,0);
		}

		// Handle Jump.
		if(Input.IsActionJustPressed("Dash")){
			dashing = true;
			return;
		}
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()){
			Velocity = new Vector3(Velocity[0], JumpVelocity,Velocity[2]);
		}
			

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
		*/

		stateMachine.process(delta);

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
	public void determineAnimationAndBroadcast(ref RecievedDataStruct pkt){
		pkt.anim = animChar;
	}
	public void setAnimationChar(char c){
		animChar = c;
	}
	public bool isOnGround(){
		return IsOnFloor();
	}
	public void broadcastPosition(ref RecievedDataStruct pkt){
		pkt.px = Position.X.ToString();
		pkt.py = Position.Y.ToString();
		pkt.pz = Position.Z.ToString();
		pkt.rotation = Rotation.ToString();
	}
	public void applyJumpForce(){
		Velocity = new Vector3(Velocity[0], JumpVelocity,Velocity[2]);
	}
	public void walk(double delta){
		Vector3 forward = Vector3.Forward;
		Vector3 side = Vector3.Left;

		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");


		forward = forward.Rotated(Vector3.Up,Rotation.Y);
		side = side.Rotated(Vector3.Up,Rotation.Y);

		forward = forward.Normalized();
		side = side.Normalized();

		float forwardMove = -inputDir[1]*moveSpeed;
		float sideMove = -inputDir[0]*moveSpeed;
		

		wishVel = side*sideMove + forward*forwardMove;

		if(IsOnFloor()){
			applyFriction(delta);
		}
		wishVel.Y = 0;

		var wishSpeed = wishVel.Length();
		var wishDir = wishVel.Normalized();

		if(wishSpeed != 0.0 && wishSpeed > maxSpeed){
			wishVel *= maxSpeed/wishSpeed;
			wishSpeed = maxSpeed;
		} 

		Accelerate(wishDir, wishSpeed, walkAccel, delta);
	}
	public void slide(){

	}
	public bool isStandingStill(){
		return Velocity.Length() <=0;
	}
	public void dash(double delta){


		//Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

	}
	public void applyGravity(double delta){
		Velocity += new Vector3(0,-fallSpeed*(float)delta,0);
	}
	public void climb(){

	}
	private void Accelerate(Vector3 wishDir, float wishSpeed, float Accel,double delta){
		//wishSpeed = Math.Min(wishSpeed, )
		float currentSpeed = Velocity.Dot(wishDir);

		float addSpeed = wishSpeed - currentSpeed;

		if(addSpeed <=0){
			return;
		}
		var accelSpeed = Accel*wishSpeed*delta;
		accelSpeed = Math.Min(accelSpeed, addSpeed);

		Velocity += Convert.ToSingle(accelSpeed)*wishDir;
	}
	private void applyFriction(double delta){
		float speed = Velocity.Length();

		if (speed<=0){
			return;
		}
		float control = speed < stopSpeed ?  stopSpeed : speed;
		var drop = control * frictionFactor * delta;

		

		var newSpeed = speed-drop;
		if(newSpeed < 0){
			newSpeed = 0;
		}
		if (newSpeed != speed){
			newSpeed/=speed;
			Velocity *= Convert.ToSingle(newSpeed);
		}
		


	}
}