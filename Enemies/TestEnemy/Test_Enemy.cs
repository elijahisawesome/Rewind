using Godot;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


public partial class Test_Enemy : BaseEnemy
{
	public const float Speed = 15.0f;
	public const float accel = 6.5f;
	public const float attackDistance = 2f;
	int armIndex = 0;
	Test_Arm[] arms;
	int visionDepth = 16;
	public int roamCounter = 1;
	public int maxRoamCounter = 4;
	public float roamRadius = 5;
	public Vector3 targetLastPosition;
	Vector3 nextPathPos;

	public NavigationAgent3D nav;
	public Player player;
	Area3D visionCone;
	Area3D proximitySensor;
	EnemyStateMachine stateMachine;
	MultiplayerManager mpm;
	public class baseTargets {
		public CharacterBody3D target;
		public int heat = 0;
		public bool marked = false;
	}
	public List<baseTargets> targets;
	CharacterBody3D target;
	bool targetAquired = false;
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
		id = (int)GetMeta("ID");
		mpm.addEnemyToTrackedMultiplayerEnemies(this,id);
		targets = new List<baseTargets>();
		
		arms = new Test_Arm[6];
		foreach(Test_Arm arm in arms){
			GD.Print(armIndex);
			arms[armIndex] = GetChild<Test_Arm>(armIndex+7);
			armIndex++;
		}
		
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
			mpm.hostTransmitEnemyState((int)GetMeta("Id"));
		}
		else{
			if(targetID != -1){
				MoveToTarget(delta);
				setArms(nextPathPos);
			}
		}
	}
    public override void setRotation(string strRot){
        String[] Rots = strRot.Split(',');
		string x = Rots[0].Remove(0,1);
		string z = Rots[2].Remove(Rots[2].Length -1,1);
		Rotation = new Vector3(x.ToFloat(), Rots[1].ToFloat(),z.ToFloat());
    }
	//check cone, add new targets if they don't exist in cone already.
	//add some threat if they're new
	public void GatherTargets(){
		try{
			var bodies = visionCone.GetOverlappingBodies();
			//var closeBodies = proximitySensor.GetOverlappingBodies();
			foreach(var body in bodies){
				bool newBod = true;
				if(body is Player){
					foreach(var tgt in targets){
						if(tgt.target == body ||(body as Player).isDead()){
							newBod = false;
							break;
						}
					}
				if(newBod){
                        baseTargets tgt = new baseTargets
                        {
                            target = (CharacterBody3D)body,
                            heat = 0
                        };
                        targets.Add(tgt);
					}
				}
			}
		}
		catch(Exception e){

		}
	}
	//add to heat based on target's distance based on targets log distance
	public int CheckVision(CharacterBody3D bod){
		var spaceState = GetWorld3D().DirectSpaceState;
		int range = 0;
		var end = bod.Position;
		var origin = new Vector3(GlobalPosition.X,GlobalPosition.Y+1f,GlobalPosition.Z);
		end = new Vector3(end.X,end.Y+1f,end.Z);
		var shot = PhysicsRayQueryParameters3D.Create(origin, end);
		shot.CollideWithAreas = false;
		shot.Exclude = excludes;
						
		var result = spaceState.IntersectRay(shot);
		

		if(result.Count > 0){
			if(result["collider"].ToString().Contains("CharacterBody3D")){
					float temp = 1/((((Vector3)result["position"]).Length() - Position.Length())/visionDepth);
					temp*=temp*50f/100;

					range = (int)temp;
				}
		}

		return range; 
	}
    public override bool hasTarget()
    {
		//GD.Print((target != null));
        return (target != null);
    }
    public override void EvaluateTargets(){
		int maxheat = 0;
		
		CharacterBody3D interimtarget = target;
		GatherTargets();
		//bool[] marked = new bool[targets.Count];
        //Check vision for all targets in cone. Add to aggro meter based on distance from target
		//remove fixed heat from all targets
		//if target heat is 0, remove from targets
		for(int x = 0; x<targets.Count; x++){
			var tgt = targets[x];
			tgt.heat += CheckVision(tgt.target);
			tgt.heat -= 2;
			if(tgt.heat > 100){
				tgt.heat = 100;
			}
			if(tgt.heat <= 0 || ((Player)tgt.target).isDead()){
				tgt.marked = true;
			}
			if(tgt.marked){
				targets.Remove(tgt);
				if(tgt.target == target){
					target = null;
				}
			}
			if(tgt.heat > maxheat){
				maxheat = tgt.heat;
				interimtarget = tgt.target;
			}

		}

		if(targets != null && targets.Any() && maxheat >0 && !((Player)interimtarget).isDead()){
			target = interimtarget;
			targetID = ((Player)target).id;
		}
		
    }
    public override bool CheckVision()
    {
	var spaceState = GetWorld3D().DirectSpaceState;
		try{
			var bodies = visionCone.GetOverlappingBodies();
			var closeBodies = proximitySensor.GetOverlappingBodies();
			foreach(var body in bodies){
				for(int x = 0; x< 2; x++){

				
					Godot.Vector3 rayTargetPos = new Godot.Vector3();
					if(body.GetType().ToString() == "Player" || body.GetType().ToString() == "MPlayer"){
						target = (CharacterBody3D)body;
						

						var end = target.Position;
						if(body.GetType().ToString() == "Player"){
							end = player.rayPos;
						}
						end = new Vector3(end.X, end.Y+x, end.Z);

						var origin = new Vector3(GlobalPosition.X,GlobalPosition.Y+1f,GlobalPosition.Z);
						end = new Vector3(end.X,end.Y+1f,end.Z);
						var shot = PhysicsRayQueryParameters3D.Create(origin, end);
						shot.CollideWithAreas = false;
						
						
						shot.Exclude = excludes;
						
						var result = spaceState.IntersectRay(shot);
						
						if(result.Count > 0){
							if(result["collider"].ToString().Contains("CharacterBody3D")){
								string res = ((CharacterBody3D)result["collider"]).GetType().ToString();
								if(res == "Player" || res == "MPlayer"){
									//var distance = origin - end;

									//testInstanceDrawBox(distance,end);
									setArms(nextPathPos);
									targetID = ((Player)target).id;
									return true;
								}
							}

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
						if(result["collider"].ToString().Contains("CharacterBody3D")){
							string res = ((CharacterBody3D)result["collider"]).GetType().ToString();
							if(res == "Player" || res == "MPlayer"){
								//var distance = origin - end;
								targetID = ((Player)target).id;
								//testInstanceDrawBox(distance,end);
								return true;
							}
						}
					}

				}
			}
		}
		catch(Exception e){
			
		}
		return false;


	}
	public void clientPathToTarget(double delta){

	}
    async void testInstanceDrawBox(Vector3 Length, Vector3 targetLocation){
		MeshInstance3D drawBox = new MeshInstance3D();
		drawBox.Mesh = new BoxMesh();
		
		
		drawBox.Scale = new Vector3(.1f,.1f,Length.Length());
		CallDeferred("add_child",drawBox);
		var sceneTree = GetTree();
		await ToSignal(sceneTree,"node_added");
		
		Vector3 Val = - drawBox.GlobalTransform.Basis.Z/2;
		//drawBox.GlobalPosition = GlobalPosition - drawBox.GlobalTransform.Basis.Z/2;
		drawBox.GlobalPosition = new Vector3(GlobalPosition.X,GlobalPosition.Y+1f,GlobalPosition.Z) + Val;
		drawBox.LookAtFromPosition(drawBox.GlobalPosition,targetLocation);

		await ToSignal(sceneTree,"node_added");
		drawBox.QueueFree();
	}
    public override void MoveToTarget(double delta)
    {
    	Godot.Vector3 velocity = Velocity;

		Godot.Vector3 dir = new Godot.Vector3();
		nav.TargetPosition = target.GlobalPosition;
		nextPathPos = nav.GetNextPathPosition();
		dir = nextPathPos - GlobalPosition;
		dir = dir.Normalized();
	
		LookAt(target.GlobalPosition);

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();
    }
	public override void setTargets(enemyMovePacket emp){
		if(emp.targetID <0){
			targetAquired = false;
			targetID = emp.targetID;
			return;
		}
		if(!targetAquired){
			targetID = emp.targetID;
			if(emp.targetID == player.id){
				target = player;
			}
			else{
				target = mpm.getMPlayer(emp.targetID);
			}
		}
	}

	public override void currentServerPos(enemyMovePacket emp){
		var serverPos = new Vector3(emp.px.ToFloat(),emp.py.ToFloat(),emp.pz.ToFloat());
		//Rotation = new Vector3(0, Rots[1].ToFloat(),0);
		if((serverPos -Position).Length() > 3){
			Position = serverPos;
		}
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
		nextPathPos = nav.GetNextPathPosition();
		dir = nextPathPos - GlobalPosition;
		dir = dir.Normalized();
		setArms(nextPathPos);
		LookAt(new Vector3(targetLastPosition.X, Position.Y,targetLastPosition.Z));

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();
    }

    public override void Attack()
    {
		Vector3 pos = new Vector3(target.Position.X,target.Position.Y+2,target.Position.Z) - Position;
		if(pos.Length() <3){
			if((target as Player).id == mpm.maxPlayerCount+1){
			(target as Player).die(pos);
		}
		else if(target is MPlayer && !(target as MPlayer).dead){
			(target as MPlayer).broadcastDeath(pos);
			(target as MPlayer).die(pos);
		}
		target = null;
		GD.Print("Hey!");
		}
		
        
    }

	public override void LookAround(double delta){
		Vector3 newRot = new Vector3(0,0,0);
		newRot.Y = (float)Math.Sin(((timer.TimeLeft/3))*delta);
		Rotation += newRot;

		setRandomArms();
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



    public override bool EvaluateAttack()
    {
        throw new NotImplementedException();
    }

	public void setArms(Vector3 pos){
		//new implementation, cast rays from sphere and place markers where rays land if they're within a certain distance. Remove markers that are too far away
		armIndex = armIndex%6;
		Random r = new Random();
		if((arms[armIndex].getMarkerPos() - nextPathPos).Length() > 3){

			var newPos = pos+(r.NextSingle()-.5f)*Vector3.One*4f;
			newPos.Y = Position.Y;
			arms[armIndex].setMarkerPos(newPos);
		}
		
		
		armIndex++;
	}
	public void setMarkerPos(Vector3 pos){
	armIndex = armIndex%6;
		Random r = new Random();
		if((arms[armIndex].getMarkerPos() - nextPathPos).Length() > 3){

			var newPos = pos+(r.NextSingle()-.5f)*Vector3.One*4f;
			newPos.Y = Position.Y;
			arms[armIndex].setMarkerPos(newPos);
		}
		armIndex++;
	}

	public Vector3 getMarkerPos(){
		return nextPathPos;
	}
	public void setRandomArms(){
		armIndex = armIndex%6;
		Random r = new Random();
		var newPos = arms[armIndex].getMarkerPos()+(r.NextSingle()-.5f)*Vector3.One*.2f;
		arms[armIndex].setMarkerPos(newPos);
		
		armIndex++;
	}
	public void retractArms(){
		foreach(var arm in arms){
			arm.setMarkerPos(Position);
		}
	}
}
