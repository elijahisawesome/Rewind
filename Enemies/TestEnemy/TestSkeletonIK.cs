using Godot;
using System;


public partial class TestSkeletonIK : SkeletonIK3D
{
	float markerDist;
	Skeleton3D parent;
	Marker3D marker;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Start();
		marker = GetParent().GetParent().GetChild<Marker3D>(0);
		markerDist = marker.Position.Length() - GetParent<Skeleton3D>().Position.Length();
		parent = GetParent<Skeleton3D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		try{
			var newDist = marker.Position.Length();
			parent.Scale = parent.Scale *newDist/markerDist;
			markerDist = newDist;
		}
		catch(Exception e){
			//GD.Print(e);
			//GetTree().Quit();
		}
		
	}
}
