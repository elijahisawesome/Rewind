using Godot;
using System;
using System.Diagnostics;


public partial class Player : CharacterBody3D
{

	public const float walkSpeed = 7.0f;
	public const float JumpVelocity = 5.5f;
	public const float airSpeed = 4f;
	public const float airAccel = 4f;
	public const float maxSpeed = 30f;
	public const float moveSpeed = 10f;
	public const float walkAccel = 10f;
	public const float fallSpeed = 10f;

	public const float stopSpeed = 1f;
	public const float frictionFactor= 3.5f;
	public Vector3 startingPosition;
	public Vector3 rayPos;
	Vector3 wishVel;
	Area3D panicBox;
	Char animChar = 'I';
	public Rid playerRid;
	public int id;
	MultiplayerManager mpm;
	StateMachine stateMachine;
	Control playerHud;
	StaminaBar playerHud_staminaBar;
	PlayerStatusFace playerHud_playerFace;
	Label deathCount;
	bool dashing;
	bool dead = false;
	himbo_base characterModel;
	CollisionShape3D col;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public override void _Ready()
    {
		mpm = GetParent<Node>().GetChild<MultiplayerManager>(2);
		characterModel = GetChild<himbo_base>(5);
		panicBox = GetChild<Area3D>(6);
		col = GetChild<CollisionShape3D>(0);
        playerRid = GetRid();
		playerHud = GetChild<Camera3D>(2).GetChild<Control>(2);
		playerHud_staminaBar = GetChild<Camera3D>(2).GetChild<Control>(2).GetChild<HBoxContainer>(1).GetChild<MarginContainer>(1).GetChild<StaminaBar>(0);
		playerHud_playerFace = GetChild<Camera3D>(2).GetChild<Control>(2).GetChild<HBoxContainer>(1).GetChild<MarginContainer>(2).GetChild<PlayerStatusFace>(0); 
		deathCount = GetChild<Camera3D>(2).GetChild<Control>(2).GetChild<HBoxContainer>(1).GetChild<MarginContainer>(0).GetChild<TextureRect>(0).GetChild<Label>(0);
		stateMachine = new StateMachine(this);
		rayPos = Position;
		startingPosition = Position;
    }
	public void noClip(){
		
	}
	public void unhideHud(){
		playerHud.Visible = true;
		//playerHud_staminaBar.Visible = true;
		//playerHud_playerFace.Visible = true;
	}
	
    public override void _PhysicsProcess(double delta)
	{
		if(!dead){
			rayPos = Position;
			stateMachine.process(delta);
		}
		else{
			stateMachine.process(delta);
		}
		
		
		MoveAndSlide();
	}
public bool isDead(){
	return dead;
}
	public void takeDamage(playerHitPacket pkt){
		//display some kinda damage indicator
		GD.Print("Shot thru the heart, and youre to blame. Uopi give loooove a bad name, da nua na an aa na na na na");
		GD.Print(pkt.damage);
		var dmgDir = mpm.getDamageDirection(pkt);
		Velocity = dmgDir*5;
		MoveAndSlide();
		
		//alter health

		//etc
	}
	public void broadcastPlayerState(){
		playerHud_playerFace.setFace(animChar);
	}
	public void takeDamage(){
		Velocity = (Transform.Basis * new Vector3(500,0,500));
		MoveAndSlide();
	}
	public async void respawn(){
		Position = startingPosition;
		await ToSignal(GetTree(),"physics_frame");
		dead = false;
		col.Disabled = false;
		stateMachine.ChangeState(stateMachine.fallingState);
		mpm.broadcastRespawn(id, Position);
	}
		public void respawn(Vector3 location){
		Position = location;
		dead = false;
		col.Disabled = false;
		stateMachine.ChangeState(stateMachine.fallingState);
	}
	public void die(Vector3 rotation){
		if(mpm.hosting){
			mpm.broadcastDeath(id, rotation);
		}
		deathCount.Text = (deathCount.Text.ToInt()+1).ToString();
		dead = true;
		col.Disabled = true;
		Node parent = GetParent();
		stateMachine.ChangeState(stateMachine.deathState);

		string armPath = "res://3D/GoreParts/himbo_base_gore_arm.tscn";
		string legPath = "res://3D/GoreParts/himbo_base_gore_legs.tscn";
		string headPath = "res://3D/GoreParts/himbo_base_gore_head.tscn";
		string torsoPath = "res://3D/GoreParts/himbo_base_gore_torso.tscn";
		PackedScene armPackedScene = GD.Load<PackedScene>(armPath);
		PackedScene legPackedScene = GD.Load<PackedScene>(legPath);
		PackedScene headPackedScene = GD.Load<PackedScene>(headPath);
		PackedScene torsoPackedScene = GD.Load<PackedScene>(torsoPath);
		var arm = armPackedScene.Instantiate<himbo_base_gore_arm>();
		var leg = legPackedScene.Instantiate<himbo_base_gore_legs>();
		var head = headPackedScene.Instantiate<himbo_base_gore_head>();
		var torso = torsoPackedScene.Instantiate<himbo_base_gore_torso>();
		arm.Position = Position;
		leg.Position = Position;
		torso.Position = Position;
		head.Position = Position;
		var knockBack = 5f;
		rotation = rotation*knockBack;
		arm.ApplyImpulse(rotation);
		head.ApplyImpulse(rotation);
		leg.ApplyImpulse(rotation);
		torso.ApplyImpulse(rotation);
		parent.CallDeferred("add_child",(arm));
		parent.CallDeferred("add_child",(leg));
		parent.CallDeferred("add_child",(torso));
		parent.CallDeferred("add_child",(head));

		
	}
	
	public void enemyHit(MPlayer target){
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
	//Consolidate moves, this is too big and annoying to look at
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
	public void airMove(double delta){
		Vector3 forward = Vector3.Forward;
		Vector3 side = Vector3.Left;

		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");


		forward = forward.Rotated(Vector3.Up,Rotation.Y);
		side = side.Rotated(Vector3.Up,Rotation.Y);

		forward = forward.Normalized();
		side = side.Normalized();

		float forwardMove = -inputDir[1]*airSpeed;
		float sideMove = -inputDir[0]*airSpeed;
		

		wishVel = side*sideMove + forward*forwardMove;

		var wishSpeed = wishVel.Length();
		var wishDir = wishVel.Normalized();

		if(wishSpeed != 0.0 && wishSpeed > maxSpeed){
			wishVel *= maxSpeed/wishSpeed;
			wishSpeed = maxSpeed;
		} 

		Accelerate(wishDir, wishSpeed, airAccel, delta);
	}
	public void NoclipMove(double delta){
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


		var wishSpeed = wishVel.Length();
		var wishDir = wishVel.Normalized();

		if(wishSpeed != 0.0 && wishSpeed > maxSpeed){
			wishVel *= maxSpeed/wishSpeed;
			wishSpeed = maxSpeed;
		} 
		applyFriction(delta);
		Accelerate(wishDir, wishSpeed, walkAccel, delta);
		deathUpAndDown();
	}
	public void deathUpAndDown(){
		if(Input.IsActionJustPressed("Jump")){
			Position = new Vector3(Position.X,Position.Y +1,Position.Z);
		}
		else if(Input.IsActionJustPressed("Crouch")){
			Position = new Vector3(Position.X,Position.Y -1,Position.Z);
		}
	}
	public bool isStandingStill(){
		return Velocity.Length() <=0;
	}
	public void panicToggle(){
		playerHud_staminaBar.panicing();
	}
	public bool panicInBurnout(){
		return playerHud_staminaBar.burnout;
	}
	public void Panic(double delta){
		Vector3 forward = Vector3.Forward;
		Vector3 side = Vector3.Left;

		forward = forward.Rotated(Vector3.Up,Rotation.Y);
		side = side.Rotated(Vector3.Up,Rotation.Y);

		forward = forward.Normalized();
		side = side.Normalized();

		float forwardMove = 2*moveSpeed;
		float sideMove = 0;
		

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
		panicBoxCheck();
		//Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
	}
	void panicBoxCheck(){
		var bods = panicBox.GetOverlappingBodies();
		foreach(var bod in bods){
			if(bod is MPlayer){
				enemyHit((bod as MPlayer));
			}
		}
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