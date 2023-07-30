using Godot;
using System;

public partial class Rewind : Node
{
	// Called when the node enters the scene tree for the first time.
	Player player;
	Camera3D camera;
	public static int REWIND_COUNT = 120;
	private int rewindPos = 0;
	private int countDown = 120;
	private bool rewinding;
	private Vector3 [] playerPositionsBuffer;
	private Vector2 [] playerRotationsBuffer;
	public override void _Ready()
	{
		
		
		playerPositionsBuffer = new Vector3[REWIND_COUNT];
		playerRotationsBuffer = new Vector2[REWIND_COUNT];
		rewinding = false;

		camera = GetParent<Camera3D>();
		player = GetParent<Camera3D>().GetParent<Player>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!rewinding && Input.IsKeyPressed(Key.P)){
			rewinding = true;
		}
		//GD.Print(playerRotationsBuffer[rewindPos]);
	}
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if(rewinding){
			rewind();
		}
		else{
			collectPositionAndRotation();
		}
    }

	void rewind(){
		player.Position = playerPositionsBuffer[rewindPos];
		camera.Rotation = new Vector3(playerRotationsBuffer[rewindPos].X,0 ,0);
		player.Rotation = new Vector3(0,playerRotationsBuffer[rewindPos].Y,0);
		
		countDown--;
		rewindPos--;
		if(countDown == 0){
			rewinding = false;
			countDown = REWIND_COUNT;
			return;
		}
		if(rewindPos ==0){
			rewindPos = 119;
		}
	}
	void collectPositionAndRotation(){
		playerPositionsBuffer[rewindPos] = player.Position;
		playerRotationsBuffer[rewindPos].X= camera.Rotation.X;
		playerRotationsBuffer[rewindPos].Y= player.Rotation.Y;
		rewindPos++;
		rewindPos %= REWIND_COUNT;
	}
}
