using Godot;
using System;
using System.Threading.Tasks;


public partial class Test_Enemy : BaseEnemy
{
	public const float Speed = 5.0f;
	public const float accel = 1.5f;

	public NavigationAgent3D nav;
	public Player player;
	Area3D area;
	EnemyStateMachine stateMachine;
	CharacterBody3D target;
	RayCast3D rayCast;
	Godot.Collections.Array<Rid> excludes;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override async void _Ready(){
		await ToSignal(GetTree(), "process_frame");
		nav = GetChild<NavigationAgent3D>(2);
		player = GetParent<Node>().GetChild<Player>(1);
		area = GetChild<Area3D>(3);
		stateMachine = new EnemyStateMachine(this);
		
		excludes = new Godot.Collections.Array<Rid>
                    {
                        GetRid()
                    };
		
	}
	public override void _PhysicsProcess(double delta)
	{
		
		stateMachine.process(delta);

	}

    public override bool CheckVision()
    {
	var spaceState = GetWorld3D().DirectSpaceState;
		try{
			var bodies = area.GetOverlappingBodies();
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
					
					GD.Print("e");
					GD.Print(rayTargetPos);
					GD.Print(end);
					GD.Print("e");
					
					shot.Exclude = excludes;
					
					var result = spaceState.IntersectRay(shot);
					
					if(result.Count > 0){
						//var collider = rayCast.GetCollider();
						//GD.Print(result["collider"]);
						string res = ((CharacterBody3D)result["collider"]).GetType().ToString();
						GD.Print(res);
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

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();
    }

    public override void CheckDistanceFromTarget()
    {
        throw new NotImplementedException();
    }

    public override void SwitchTarget()
    {
        //throw new NotImplementedException();
    }

    public override void MoveToLastKnownTargetPosition()
    {
        throw new NotImplementedException();
    }

    public override void Attack()
    {
        throw new NotImplementedException();
    }

    public override void Roam()
    {
        throw new NotImplementedException();
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
