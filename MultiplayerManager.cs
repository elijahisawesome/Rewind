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
	public int clientNumber = 1;
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
	public void hostTransmitEnemyState(int id){

	}
	private void spawnHost(){
		
		string path = "res://OtherPlayers/MPlayer.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
		host = packedScene.Instantiate<MPlayer>();
		host.Position = new Vector3(0,15,0);
		host.id = maxPlayerCount+1;
		host.hostPort = 11111;
		host.port = 22222;

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
	}
		public override async void _PhysicsProcess(double delta)
	{
		if(UI.isConnected() && udpClient != null){
			try{
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber = clientNumber;
				player.broadcastPosition(ref packet);
				player.determineAnimationAndBroadcast(ref packet);

				connection.clientAcceptGenericTCPSignal();
				
				
				clientSend.sendData(packet,clientSend.RemoteIpEndPoint);
			
				RecievedDataStruct returnPacket = new RecievedDataStruct();
				playerHitPacket returnHitPacket = new playerHitPacket();
				await hostRecieve.RecieveData();
				if(hostRecieve.packetType == 'm'){
					returnPacket = hostRecieve.getMovePacket();
					if(returnPacket.clientNumber > maxPlayerCount){
					host.setOrientation(returnPacket);
					}
				}
				else if(hostRecieve.packetType == 'd'){
					returnHitPacket = hostRecieve.getHitPacket();
					if(returnHitPacket.recieverID == player.id){
						player.takeDamage(returnHitPacket);
					}
				}
				
			}
			catch(Exception e){
				GD.PrintErr(e);
			}


		}
		else if(UI.isHosting()){
			if(playerSpawnQueue){
				GD.Print(mPlayers[0]);
				GD.Print(playerCount);
				playerSpawnQueue = false;
			}
			//Fix this shit later
			//Get Orientation of all clients
			for(int x = 0; x<playerCount; x++){
				await mPlayers[x].recieveUDPPacket();
				char packetType = mPlayers[x].getPacketType();
				if(packetType == 'm'){
					mPlayers[x].recieveOrientation();
				}
				else if(packetType == 'd'){
					mPlayers[x].recieveDamage();
				}
			}
			//Broadcast Orientation of self and all clients to all other clients
			for(int x = 0; x<playerCount; x++){
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber=x;
				packet.px = mPlayers[x].Position.X.ToString();
				packet.py = mPlayers[x].Position.Y.ToString();
				packet.pz = mPlayers[x].Position.Z.ToString();
				packet.rotation = mPlayers[x].Rotation.ToString();
			}
			for(int x = 0; x<playerCount; x++){
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber = clientNumber;
				player.determineAnimationAndBroadcast(ref packet);
				player.broadcastPosition(ref packet);
				mPlayers[x].hostTransmitPositionToPlayers(packet);
			}
		}

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
		clientNumber = maxPlayerCount+1;
		player.id = maxPlayerCount+1;
		connection = new TCPConnection(true, this);
		await connection.Connect("");
	}
	public void setPlayerAndMPMID(int id){
		player.id = id;
		clientNumber = id;
	}

	private void welcomeNewPlayer(MPlayer newPlayer){
		connection.sendNewPlayerPortAndID(newPlayer, player.Position);
	}
	private void broadcastConnect(){
		for(int x = 0; x<playerCount; x++){
			
		}
	}
	public void broadcastDeath(int id, Vector3 rotation){
		connection.serverSendClientDeath(id, rotation);
	}
	public void playerHit(int hitPlayersID, int attackersID){
		if(hosting){
			for(int x = 0; x<playerCount; x++){
				GD.Print("SHOOTING");
				playerHitPacket packet = new playerHitPacket();
				packet.attackerID = attackersID;
				packet.recieverID = hitPlayersID;
				packet.damage = 20;
				mPlayers[x].transmitDamageToPlayers(packet);
			}
		}
		else{
			playerHitPacket packet = new playerHitPacket();
			packet.attackerID = attackersID;
			packet.recieverID = hitPlayersID;
			packet.damage = 20;
			clientSend.sendData(packet,clientSend.RemoteIpEndPoint);
		}

	}
}
