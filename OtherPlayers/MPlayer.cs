using Godot;
using System;
using System.Diagnostics;


public partial class MPlayer : CharacterBody3D
{
	public int id;
	UDPSend send;
	UDPRecieve recieve;
	System.Net.Sockets.UdpClient client;
	Player localPlayer;
	himbo_base characterModel;
	CollisionShape3D collider;
	public int port;
	public int hostPort;
	public Vector3 Pos;
	public Vector3 Rot;
	public Rid playerID;
	public int goreMeshCount = 4;
	char packetType = '\0';
	public bool dead;
	private Node parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dead = false;
		playerID = GetRid();
		localPlayer = GetNode<Player>("../Player");
		collider = GetChild<CollisionShape3D>(0);
		parent = GetParent();
		characterModel = GetChild<MeshInstance3D>(1).GetChild<himbo_base>(0);
		
	}
	public override void _PhysicsProcess(double dleta){
		MoveAndSlide();
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
	public void die(){
		//this.Visible = false;

		//TODO: change this, make several bodies spawn attached to the root node, fire em off and then remove them after a timer. Remove the player's collision as well.
		dead = true;
		collider.Disabled = true;
		this.Visible = false;
		spawnGore();
	}
	private void spawnGore(){
		string armPath = "res://3D/GoreParts/himbo_base_gore_arm.tscn";
		string legPath = "res://3D/GoreParts/himbo_base_gore_legs.tscn";
		string headPath = "res://3D/GoreParts/himbo_base_gore_head.tscn";
		string torsoPath = "res://3D/GoreParts/himbo_base_gore_torso.tscn";
		PackedScene armPackedScene = GD.Load<PackedScene>(armPath);
		PackedScene legPackedScene = GD.Load<PackedScene>(legPath);
		PackedScene headPackedScene = GD.Load<PackedScene>(headPath);
		PackedScene torsoPackedScene = GD.Load<PackedScene>(torsoPath);
		var arm = armPackedScene.Instantiate<himbo_base_gore_arm>();
		var leg = legPackedScene.Instantiate<himbo_base_gore_legs>();
		var head = headPackedScene.Instantiate<himbo_base_gore_head>();
		var torso = torsoPackedScene.Instantiate<himbo_base_gore_torso>();
		arm.Position = Position;
		leg.Position = Position;
		torso.Position = Position;
		head.Position = Position;
		arm.ApplyCentralImpulse(new Vector3(1,10,1));
		head.ApplyCentralImpulse(new Vector3(10,40,1));
		leg.ApplyCentralImpulse(new Vector3(10,40,1));
		torso.ApplyCentralImpulse(new Vector3(1,10,1));
		parent.CallDeferred("add_child",(arm));
		parent.CallDeferred("add_child",(leg));
		parent.CallDeferred("add_child",(torso));
		parent.CallDeferred("add_child",(head));
	}
}
