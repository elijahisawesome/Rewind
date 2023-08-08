using Godot;
using System;
using System.Threading;
using System.Net.Sockets;

public partial class MultiplayerManager : Node
{
	Player localPlayer;
	MPlayer [] mPlayers;
	MPlayer host;
	int playerCount = 0;
	int maxPlayerCount = 4;
	int defaultUDPPort = 11003;
	UdpClient udpClient;
	UdpClient udpClientHost;
	// Called when the node enters the scene tree for the first time.
		//Test Networking;
	////////////
	public UDPSend clientSend;
	public UDPRecieve hostRecieve;
	public ConnectScreenUI UI;
	public string hostAddress;
	TCPConnection connection;
	Player player;

	public bool hosting;
	private bool playerSpawnQueue = false;
	public bool readyToStream = false;
	public override void _Ready()
	{	
		mPlayers = new MPlayer[maxPlayerCount];
		UI = GetParent<Node>().GetNode<ConnectScreenUI>("Player/ConnectScreenUI");
		player = GetParent<Node>().GetNode<Player>("Player");
	}
	public async void connectClient(string adr){
		connection = new TCPConnection(false, this);
		await connection.Connect(adr);
		spawnHost();
	}
	private void spawnHost(){
		
		string path = "res://OtherPlayers/MPlayer.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
		host = packedScene.Instantiate<MPlayer>();
		host.Position = new Vector3(0,15,0);

		CallDeferred("add_child",(host));
		GD.Print("Host spawns");
	}
	public void setClientUDP(int newUDPPort){
			GD.Print("PORT");
			GD.Print(newUDPPort);
			GD.Print("PORT");
			udpClient = new UdpClient();
			udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

					hostRecieve = new UDPRecieve(ref udpClient);
		hostRecieve.Connect("",newUDPPort);
	}
	public void setExistingPlayers(){
		
	}
	public void setHostAddress(string hA){
		hostAddress = hA;
	}
	public void setClientUDPHost(int newUDPPort){
			GD.Print("Local");
			GD.Print(newUDPPort);
			GD.Print("Local");

		
			clientSend = new UDPSend(ref udpClient);
			clientSend.Connect(hostAddress, newUDPPort);

		//udpClientHost = new UdpClient(newUDPPort);
		//hostRecieve = new UDPRecieve(ref udpClientHost);
	}
		public override async void _PhysicsProcess(double delta)
	{
		if(UI.isConnected() && udpClient != null){
			try{
				
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.px = player.Position.X.ToString();
				packet.py = player.Position.Y.ToString();
				packet.pz = player.Position.Z.ToString();
				clientSend.sendData(packet,clientSend.RemoteIpEndPoint);
			
				RecievedDataStruct returnPacket = new RecievedDataStruct();
				await hostRecieve.RecieveData();
				returnPacket = hostRecieve.getPacket();
				if(returnPacket.clientNumber > maxPlayerCount){
				host.setOrientation(returnPacket);
			}
			}
			catch(Exception e){
				GD.PrintErr(e);
			}


		}
		else if(UI.isHosting()){
			if(playerSpawnQueue){
				//CallDeferred("add_child",(mPlayers[0]));
				GD.Print(mPlayers[0]);
				GD.Print(playerCount);
				playerSpawnQueue = false;
			}
			try{
				RecievedDataStruct packet = new RecievedDataStruct();
				await hostRecieve.RecieveData();
				packet = hostRecieve.getPacket();
			}
			catch(Exception e){

			}

			//Fix this shit later
			//Get Orientation of all clients
			for(int x = 0; x<playerCount; x++){
				await mPlayers[x].recieveOrientation();
				//mPlayers[x].setOrientation();
			}
			//Broadcast Orientation of self and all clients to all other clients
			for(int x = 0; x<playerCount; x++){
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber=x;
				packet.px = mPlayers[x].Position.X.ToString();
				packet.py = mPlayers[x].Position.Y.ToString();
				packet.pz = mPlayers[x].Position.Z.ToString();
				packet.rotation = mPlayers[x].Rotation.ToString();
				//Godot.GD.Print("Hey!");
				//Godot.GD.Print(mPlayers[x].ToString());
				//GD.Print(mPlayers[x].port);
				//mPlayers[x].hostTransmitPositionToPlayers(packet);
			}
			for(int x = 0; x<playerCount; x++){
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber = maxPlayerCount+1;
				packet.px = player.Position.X.ToString();
				packet.py = player.Position.Y.ToString();
				packet.pz = player.Position.Z.ToString();
				packet.rotation = player.Rotation.ToString();
				mPlayers[x].hostTransmitPositionToPlayers(packet);
			}
		}

	}
	public static Vector3 GetPositionFromPacket(RecievedDataStruct packet){
			Vector3 Pos = new Vector3();
				Pos[0] = packet.px.ToFloat();
				Pos[1] = packet.py.ToFloat();
				Pos[2] = packet.pz.ToFloat();
				return Pos;
	}
	public int getHostUDPPort(){
		return defaultUDPPort;
	}
    //When connection is recieved, spawn new mPlayer;
	public void playerConnect(string newPlayerAddress){
		string path = "res://OtherPlayers/MPlayer.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
		MPlayer newPlayer = packedScene.Instantiate<MPlayer>();
		if(playerCount < maxPlayerCount){
			newPlayer.id = playerCount;
			newPlayer.port = defaultUDPPort+1+playerCount;
			newPlayer.hostPort = defaultUDPPort;
			mPlayers[playerCount] = newPlayer;
			//mPlayers[playerCount].id = playerCount;
			
			//GetParent<Node>().CallDeferred("add_child",mPlayers[playerCount]);
			CallDeferred("add_child",(mPlayers[playerCount]));
			GD.Print("Player COnnected");
			playerCount++;
			playerSpawnQueue = true;
		}
		if(hosting){
			welcomeNewPlayer(newPlayer);
			broadcastConnect();
			newPlayer.setRecieve(newPlayerAddress);
		}

	}
	public async void setHosting(){
		hosting = true;
		connection = new TCPConnection(true, this);
		await connection.Connect("");
	}

	//Send all relavent info to all mPlayers;
	private void broadcast(){

	}
	private void welcomeNewPlayer(MPlayer newPlayer){
		connection.sendNewPlayerPortAndID(newPlayer, player.Position);
	}
	private void broadcastConnect(){
		for(int x = 0; x<playerCount; x++){
			
		}
	}
	//public void 
}
