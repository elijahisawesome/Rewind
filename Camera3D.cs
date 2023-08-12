using Godot;
using System;

public partial class Camera3D : Godot.Camera3D
{
	ConnectScreenUI UI;
	// Called when the node enters the scene tree for the first time.
	private float prevX;
	private float prevY;
	private float mouseAccel = .035f;
	private bool readyToMove;
	public bool fireCheck;
	public override void _Ready()
	{
		//GetViewport().GetMousePosition();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Vector2 newCoords = GetViewport().GetMousePosition();
		prevX = newCoords.X;
		prevY = newCoords.Y;
		UI = GetParent<Player>().GetNode<ConnectScreenUI>("ConnectScreenUI");
		readyToMove = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(UI);
		if(UI.isConnected()||UI.isHosting()){
			readyToMove = true;
		}
		if(Input.IsKeyPressed(Key.Escape)){
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		//GatherMotion();
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if(@event is InputEventMouseMotion&&readyToMove){
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			RotateX(-mouseEvent.Relative.Y * mouseAccel);
			GetParent<Player>().RotateY(-mouseEvent.Relative.X * mouseAccel);
		}
		if(@event is InputEventMouseButton && Input.MouseMode == Input.MouseModeEnum.Visible){
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if(@event is InputEventMouseButton && @event.IsPressed()){
			fireCheck = true;
		}
	}
}
