using Godot;
using System;

public partial class Gun : Node3D
{
	public Player player;
	public RayCast3D ray;
	private Camera3D camera;
	const int MAXRAYLENGTH = 500;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Player>();
		camera = GetParent<Player>().GetChild<Camera3D>(2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public override void _PhysicsProcess(double delta){
		if(camera.fireCheck){
			var spaceState = GetWorld3D().DirectSpaceState;

			var mousePos = GetViewport().GetMousePosition();
			var origin = camera.ProjectRayOrigin(mousePos);
			var end = origin + camera.ProjectRayNormal(mousePos) * MAXRAYLENGTH;
			var shot = PhysicsRayQueryParameters3D.Create(origin, end);

			shot.CollideWithAreas = true;

			shot.Exclude = new Godot.Collections.Array<Rid>{player.playerRid};
			
			var result = spaceState.IntersectRay(shot);

			try{
				if(result.Count > 0){
					var pp = (MPlayer)result["collider"];
					//MPlayer mI = pp.GetParent<MPlayer>();
					player.enemyHit(pp);
					//mI.QueueFree();
					//Rid ridd = new Rid(result["rid"]);
					//GD.Print(ListOfMPlayers.checkRids(result["rid"]));

				}
			}
			catch(Exception e){
				GD.PrintErr(e);
			}
			
			
			camera.fireCheck = false;
		}


		//Godot.PhysicsDirectSpaceState3D spaceState = 
			}
}
