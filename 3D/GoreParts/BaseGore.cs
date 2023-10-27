using Godot;
using System;

public partial class BaseGore : RigidBody3D
{
	float removeTimer = 5;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		string bloodPath = "res://2DArt/BloodSplatters/BloodParticles.tscn";
		PackedScene bloodPackedScene = GD.Load<PackedScene>(bloodPath);
		var blood = bloodPackedScene.Instantiate<BloodParticles>();
		blood.Position = new Vector3(Position.X,Position.Y+1,Position.Z);
		GetChild<CollisionShape3D>(0).CallDeferred("add_child", blood);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(removeTimer <0){
			GetParent().RemoveChild(this);
			QueueFree();
			this.Dispose();
		}
		removeTimer -= (float)delta;
	}

}
