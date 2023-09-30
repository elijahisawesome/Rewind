using Godot;
using System;

public partial class Test_Arm : Node3D
{
	Marker3D marker;
	public float markerPos;
	Vector3 targetPos;
	AudioStreamPlayer3D audioStream;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		marker = GetChild<Node3D>(0).GetChild<Marker3D>(0);
		markerPos = marker.Position.Y;
		audioStream = GetChild<AudioStreamPlayer3D>(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void setMarkerPos(Vector3 newPos){
		//marker.Position = new Vector3(marker.Position.X,newPos.X,marker.Position.Z);
		targetPos = newPos;
		marker.GlobalPosition = newPos;
		audioStream.Play();
		//GD.Print(newPos);
		//GD.Print(GlobalPosition-newPos);
	}
	public Vector3 getMarkerPos(){
		return targetPos;
	}
}
