using Godot;
using System;
using System.Diagnostics;


public partial class MPlayer : MeshInstance3D
{
	public int id;
	UDPSend send;
	UDPRecieve recieve;
	System.Net.Sockets.UdpClient client;
	Player localPlayer;
	himbo_base characterModel;
	public int port;
	public int hostPort;
	public Vector3 Pos;
	public Vector3 Rot;
	public Rid playerID;
	char packetType = '\0';
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector3(3,15,3);
		playerID = GetChild<CharacterBody3D>(1).GetRid();
		localPlayer = GetNode<Player>("../../Player");
		characterModel = GetChild<himbo_base>(2);
	}
	public void setRecieve(string IP){
		client = new System.Net.Sockets.UdpClient();
		client.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);


		recieve = new UDPRecieve(ref client);
		send = new UDPSend(ref client);
		GD.Print("hostPort");
		GD.Print(hostPort);
		GD.Print(port);
		GD.Print("port");
		recieve.Connect(IP, hostPort);
		send.Connect(IP, port);
	}
	public override void _Process(double delta)
	{
	}

	public void hostTransmitPositionToPlayers(RecievedDataStruct str){
		send.sendData(str, send.RemoteIpEndPoint);
	}
	public void transmitDamageToPlayers(playerHitPacket packet){
		send.sendData(packet, send.RemoteIpEndPoint);
	}
	public char getPacketType(){
		return packetType;
	}
	public async System.Threading.Tasks.Task recieveUDPPacket(){
		await recieve.RecieveData();
		packetType = recieve.packetType;
		recieve.resetPacketType();

//		
	}
	public void recieveOrientation(){
		var packet = recieve.getMovePacket();
		setOrientation(packet);
	}
	public void recieveDamage(){
		var packet = recieve.getHitPacket();
		setDamage(packet);
	}
	public void setOrientation(RecievedDataStruct packet){
		try{
			setHimboAnimation(packet.anim);
			Vector3 pos = new Vector3(packet.px.ToFloat(),packet.py.ToFloat(),packet.pz.ToFloat());
			//Vector3 rot = packet.rotation;
			extractRotation(packet.rotation);
			Position = pos;
			//GD.Print(new Vector3(packet.px.ToFloat(),packet.py.ToFloat(),packet.pz.ToFloat()));
		}
		catch(Exception e){
			GD.PrintErr(e);
		}

	}
	private void extractRotation(String strRot){
		String[] Rots = strRot.Split(',');

		Rotation = new Vector3(0, Rots[1].ToFloat(),0);
		
	}
	private void setHimboAnimation(char animChar){
		characterModel.swtichAnimation(animChar);
	}
	void setDamage(playerHitPacket pkt){
		//remove hard coding and add code for other players
		if(pkt.recieverID == 5){
			localPlayer.takeDamage(pkt);
		}
	}
}
