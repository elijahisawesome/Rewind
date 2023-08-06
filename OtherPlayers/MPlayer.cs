using Godot;
using System;

public partial class MPlayer : MeshInstance3D
{
	public int id;
	UDPSend send;
	UDPRecieve recieve;
	System.Net.Sockets.UdpClient client;
	public int port;
	public Vector3 Pos;
	public Vector3 Rot;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector3(0,15,0);
		
	}
	public void setRecieve(string IP){
		client = new System.Net.Sockets.UdpClient();
		recieve = new UDPRecieve(ref client);
		send = new UDPSend(ref client);
		recieve.Connect(IP, port);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void hostTransmitPositionToPlayers(RecievedDataStruct str){
		//send.sendData(str);
	}
	public async System.Threading.Tasks.Task recieveOrientation(){
		await recieve.RecieveData();
		var packet = recieve.getPacket();

/*
		GD.Print(packet.clientNumber);
		GD.Print(packet.px);
		GD.Print(packet.py);
		GD.Print(packet.pz);
*/
		setOrientation(packet);
	}
	public void setOrientation(RecievedDataStruct packet){
		try{
			Vector3 pos = new Vector3(packet.px.ToFloat(),packet.py.ToFloat(),packet.pz.ToFloat());
			Position = pos;
		}
		catch(Exception e){
			GD.PrintErr(e);
		}

	}
}
