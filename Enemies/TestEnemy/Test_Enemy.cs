using Godot;
using System;
using System.Threading.Tasks;


public partial class Test_Enemy : BaseEnemy
{
	public const float Speed = 5.0f;
	public const float accel = 1.5f;
	public const float attackDistance = 2f;

	public int roamCounter = 1;
	public int maxRoamCounter = 4;
	public float roamRadius = 5;
	public Vector3 targetLastPosition;

	public NavigationAgent3D nav;
	public Player player;
	Area3D visionCone;

	Area3D proximitySensor;
	EnemyStateMachine stateMachine;
	MultiplayerManager mpm;
	CharacterBody3D target;
	RayCast3D rayCast;
	Timer timer;
	Godot.Collections.Array<Rid> excludes;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override async void _Ready(){
		await ToSignal(GetTree(), "process_frame");
		nav = GetChild<NavigationAgent3D>(2);
		player = GetParent<Node>().GetChild<Player>(1);
		visionCone = GetChild<Area3D>(3);
		proximitySensor = GetChild<Area3D>(4);
		timer = GetChild<Timer>(5);
		mpm = GetNode<MultiplayerManager>("%MultiplayerManager");
		
		
		stateMachine = new EnemyStateMachine(this);

		
		excludes = new Godot.Collections.Array<Rid>
                    {
                        GetRid()
                    };
		targetLastPosition = new Vector3();
		
	}
	public override void _PhysicsProcess(double delta)
	{
		if(mpm.hosting){
			stateMachine.process(delta);
			mpm.hostTransmitEnemyState((int)GetMeta("ID"));
		}
		

	}

    public override bool CheckVision()
    {
	var spaceState = GetWorld3D().DirectSpaceState;
		try{
			var bodies = visionCone.GetOverlappingBodies();
			var closeBodies = proximitySensor.GetOverlappingBodies();
			foreach(var body in bodies){
				Godot.Vector3 rayTargetPos = new Godot.Vector3();
				if(body.GetType().ToString() == "Player" || body.GetType().ToString() == "MPlayer"){
					target = (CharacterBody3D)body;
					

					var end = target.Position;
					if(body.GetType().ToString() == "Player"){
						end = player.rayPos;
					}

					var origin = GlobalPosition;
					
					var shot = PhysicsRayQueryParameters3D.Create(origin, end);
					shot.CollideWithAreas = false;
					
					
					shot.Exclude = excludes;
					
					var result = spaceState.IntersectRay(shot);
					
					if(result.Count > 0){
						
						string res = ((CharacterBody3D)result["collider"]).GetType().ToString();
						
						if(res == "Player" || res == "MPlayer"){
							
							return true;
						}
					}
					
				}
			}
		foreach(var body in closeBodies){
			Godot.Vector3 rayTargetPos = new Godot.Vector3();
				if(body.GetType().ToString() == "Player" || body.GetType().ToString() == "MPlayer"){
					target = (CharacterBody3D)body;

					var end = target.Position;
					if(body.GetType().ToString() == "Player" || body.GetType().ToString() == "Player"){
						end = player.rayPos;
					}

					var origin = GlobalPosition;
					
					var shot = PhysicsRayQueryParameters3D.Create(origin, end);
					shot.CollideWithAreas = false;
					
					
					shot.Exclude = excludes;
					
					var result = spaceState.IntersectRay(shot);
					
					if(result.Count > 0){
						//var collider = rayCast.GetCollider();
						//GD.Print(result["collider"]);
						string res = ((CharacterBody3D)result["collider"]).GetType().ToString();
						if(res == "Player" || res == "MPlayer"){
							return true;
						}
					}

				}
			}
		}
		catch(Exception e){

		}
		return false;
    }

    public override void MoveToTarget(double delta)
    {
    	Godot.Vector3 velocity = Velocity;

		Godot.Vector3 dir = new Godot.Vector3();
		nav.TargetPosition = target.GlobalPosition;
		
		dir = nav.GetNextPathPosition() - GlobalPosition;
		dir = dir.Normalized();
	
		LookAt(target.GlobalPosition);

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();
    }

    public override void CheckDistanceFromTarget()
    {
        throw new NotImplementedException();
    }
	public override void SetTargetLastPosition(){
		targetLastPosition = target.GlobalPosition;
	}
	public override bool HasArrivedAtLastPosition(){
		return (Position-targetLastPosition).Length()<=attackDistance;
	}
    public override void SwitchTarget()
    {
        //throw new NotImplementedException();
    }

    public override void MoveToLastKnownTargetPosition(double delta)
    {
        Godot.Vector3 velocity = Velocity;

		Godot.Vector3 dir = new Godot.Vector3();
		nav.TargetPosition = targetLastPosition;
		
		dir = nav.GetNextPathPosition() - GlobalPosition;
		dir = dir.Normalized();
	
		LookAt(new Vector3(targetLastPosition.X, Position.Y,targetLastPosition.Z));

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();
    }

    public override void Attack()
    {
		if(target is Player){
			(target as Player).die(Rotation);
		}
		else if(target is MPlayer && !(target as MPlayer).dead){
			(target as MPlayer).broadcastDeath(Rotation);
			(target as MPlayer).die(Rotation);
		}
		GD.Print("Hey!");
        
    }

	public override void LookAround(double delta){
		Vector3 newRot = new Vector3(0,0,0);
		newRot.Y = (float)Math.Sin(((timer.TimeLeft/3))*delta);
		Rotation += newRot;


	}
	public override bool SearchTimerComplete(){
		if(timer.TimeLeft < 0.1f){
			timer.Stop();
			return true;
		}
		return false;
	}

	public override bool DoneSearching(){

		return roamCounter >=maxRoamCounter;
	}
	public override void StartTimer(){
		timer.Start();
	}

    public override void Roam()
    {
        GD.Print("lookin around, bud");
    }
	public override void IterateRoamCounter(){
		roamCounter++;
	}
	public override void ResetRoamCounter(){
		roamCounter = 1;
	}
	public override void MoveToRoamTarget(double delta){
		MoveToLastKnownTargetPosition(delta);
	}
	public override bool SetRoamTarget(){
		Random rnd = new Random();
		var state = GetWorld3D().DirectSpaceState;
		float factorX = (rnd.NextSingle() - .5f)*roamRadius;
		float factorY = 0;
		float factorZ = (rnd.NextSingle() - .5f)*roamRadius;

		Vector3 testTargetLastPosition = new Vector3(targetLastPosition.X +factorX, targetLastPosition.Y +factorY,targetLastPosition.Z +factorZ);
		var shot = PhysicsRayQueryParameters3D.Create(Position, testTargetLastPosition);
		shot.Exclude = excludes;
					
		var result = state.IntersectRay(shot);
		if(result.Count > 0){
			return false;
		}
		targetLastPosition = testTargetLastPosition;
		return true;
	}

    public override void EvaluateTargets()
    {
        throw new NotImplementedException();
    }

    public override bool EvaluateAttack()
    {
        throw new NotImplementedException();
    }

}
