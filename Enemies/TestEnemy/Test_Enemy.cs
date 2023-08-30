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
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override async void _Ready(){
		await ToSignal(GetTree(), "process_frame");
		nav = GetChild<NavigationAgent3D>(2);
		player = GetParent<Node>().GetChild<Player>(1);
		area = GetChild<Area3D>(3);

		stateMachine = new EnemyStateMachine(this);
		
	}
	public override void _PhysicsProcess(double delta)
	{
		stateMachine.process(delta);
		/*Vector3 velocity = Velocity;

		Vector3 dir = new Vector3();
		nav.TargetPosition = player.GlobalPosition;
		dir = nav.GetNextPathPosition() - GlobalPosition;
		dir = dir.Normalized();

		velocity = velocity.Lerp(dir*Speed, accel*(float)delta);
		
		Velocity = velocity;
		
		MoveAndSlide();*/
	}

    public override bool CheckVision()
    {
		
		var bodies = area.GetOverlappingBodies();
		foreach(var body in bodies){
			if(body.GetType().ToString() == "Player" || body.GetType().ToString() == "MPlayer"){
				target = (CharacterBody3D)body;
			}
		}

		return false;
    }

    public override void MoveToTarget()
    {
        throw new NotImplementedException();
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
