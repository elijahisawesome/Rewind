using Godot;
using System;

public partial class BloodSplatter : Sprite3D
{	
	float rotationRate;
	float removeTimer = 5;
	Vector3 transPosedRotation;
	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		transPosedRotation = new Vector3();
		rotationRate = (1/(new Random().NextSingle()*5)%5)/4;
		player = GetNode<Node>("/root").GetChild(0).GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if(removeTimer <0){
			GetParent().RemoveChild(this);
			this.Dispose();
		}
		removeTimer -= (float)delta;
		GD.Print(Math.Cos(Math.PI*removeTimer/10));
		Rotation += new Vector3(1*rotationRate, 1 *rotationRate,1 *rotationRate);
		Position = new Vector3(Position.X,Position.Y+(float)Math.Cos(Math.PI*removeTimer/2.5)*rotationRate,Position.Z);
	}
}
