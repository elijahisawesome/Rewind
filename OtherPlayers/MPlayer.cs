using Godot;
using System;
using System.Diagnostics;
using System.Net.Sockets;
//using System.Numerics;


public partial class MPlayer : Player
{
	//public int id;
	public UDPSend send;
	public UDPRecieve recieve;
	TcpClient tcpClient;
	System.Net.Sockets.UdpClient client;
	Player localPlayer;
	himbo_base characterModel;
	CollisionShape3D collider;
	MultiplayerManager mpm;
	public Vector3 rayPos;
	public int port;
	public int hostPort;
	public Vector3 Pos;
	public Vector3 Rot;
	public Rid playerID;
	public int goreMeshCount = 4;
	char packetType = '\0';
	public bool dead;
	private Node parent;
	public char anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dead = false;
		playerID = GetRid();
		localPlayer = GetNode<Player>("../../Player");
		collider = GetChild<CollisionShape3D>(0);
		parent = GetParent();
		characterModel = GetChild<MeshInstance3D>(1).GetChild<himbo_base>(0);
		mpm = GetParent<MultiplayerManager>();
	}
	public override void _PhysicsProcess(double dleta){
		rayPos = Position;
		MoveAndSlide();
	}
	public void setTCPClient(TcpClient cl){
		tcpClient = cl;
	}
	public TcpClient getTCPClient(){
		return tcpClient;
	}
	public void setRecieve(string IP){
		client = new System.Net.Sockets.UdpClient();
		client.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);


		recieve = new UDPRecieve(ref client);
		send = new UDPSend(ref client);
		recieve.Connect(IP, hostPort);
		send.Connect(IP, port);
	}
	public override void _Process(double delta)
	{
	}
	public void sendNewPlayer(){
		
	}

	public void hostTransmitPositionToPlayers(RecievedDataStruct str){
		send.sendData(str, send.RemoteIpEndPoint);
	}
	public void hostTransmitPositionToPlayers(enemyMovePacket str){
		send.sendData(str, send.RemoteIpEndPoint);
	}
	public void transmitDamageToPlayers(playerHitPacket packet){
		send.sendData(packet, send.RemoteIpEndPoint);
	}
	public void transmitDeathToPlayers(){

	}
	public void flushUDPPacket(){
		send.flushUDPPacket();
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
			Position = pos;
			extractRotation(packet.rotation);
			
			//GD.Print(new Vector3(packet.px.ToFloat(),packet.py.ToFloat(),packet.pz.ToFloat()));
		}
		catch(Exception e){
			GD.PrintErr(e);
		}

	}

	public string[] getPresplitPacket(){
		return recieve.getPresplitPacket();
	}
	private void extractRotation(String strRot){
		String[] Rots = strRot.Split(',');

		Rotation = new Vector3(0, Rots[1].ToFloat(),0);
		
	}
	private void setHimboAnimation(char animChar){
		anim = animChar;
		characterModel.swtichAnimation(animChar);
	}
	void setDamage(playerHitPacket pkt){
		//remove hard coding and add code for other players
		if(pkt.recieverID == 5){
			localPlayer.takeDamage(pkt);
		}
	}
	public void respawn(Vector3 location){
		dead = false;
		collider.Disabled = false;
		this.Visible = true;
		Position = location;
	}
	public void die(Godot.Vector3 rotation){
		//this.Visible = false;
		
		dead = true;
		collider.Disabled = true;
		this.Visible = false;
		spawnGore(rotation);
	}
	public void broadcastDeath(Godot.Vector3 rotation){
		mpm.broadcastDeath(id,rotation);
	}
	private void spawnGore(Godot.Vector3 rotation){
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
		var knockBack = 5f;
		rotation = rotation*knockBack;
		arm.ApplyImpulse(rotation);
		head.ApplyImpulse(rotation);
		leg.ApplyImpulse(rotation);
		torso.ApplyImpulse(rotation);
		parent.CallDeferred("add_child",(arm));
		parent.CallDeferred("add_child",(leg));
		parent.CallDeferred("add_child",(torso));
		parent.CallDeferred("add_child",(head));
	}
	public void destroy(){
		mpm.ClosedConnection(this);
		mpm.RemoveChild(this);
		
		this.Dispose();
	}
}
