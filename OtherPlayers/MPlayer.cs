using Godot;
using System;

public partial class MPlayer : MeshInstance3D
{
	public int id;
	UDPSend send;
	UDPRecieve recieve;

	public Vector3 Position;
	public Vector3 Rotation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void hostTransmitPositionToPlayers(RecievedDataStruct str){
		send.sendData(str);
	}
	public async System.Threading.Tasks.Task recieveOrientation(){
		await recieve.RecieveData();
		var packet = recieve.getPacket();
		setOrientation(packet);
	}
	public void setOrientation(RecievedDataStruct packet){
		GD.Print(packet);
	}
}
