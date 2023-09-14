using Godot;
using System;

public partial class BloodParticles : GpuParticles3D
{	
	float removeTimer = 5;
	Node3D parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent= GetParent<Node3D>();
		GlobalPosition = parent.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		
	}
}