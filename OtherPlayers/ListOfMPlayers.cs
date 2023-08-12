using Godot;
using System;

public partial class ListOfMPlayers : Node3D
{
	public static Rid[] mPlayerIds = new Rid[10];
	private static int pos = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public static void setRid(Rid newRid){
		mPlayerIds[pos] = newRid;
		pos++;
	}
	public static Rid checkRids(Rid toBeChecked){
			for(int x = 0; x > pos; x++){
				if(mPlayerIds[x] == toBeChecked){
					return toBeChecked;
				}
			}
		return new Rid();
	}
}
