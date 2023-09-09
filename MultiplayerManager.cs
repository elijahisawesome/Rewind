using Godot;
using System;
using System.Threading;
using System.Net.Sockets;

public partial class MultiplayerManager : Node
{
	Player localPlayer;
	MPlayer [] mPlayers;
	MPlayer host;
	public int playerCount = 0;
	public int enemyCount = 0;
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

	public BaseEnemy[] enemies;

	public bool hosting;
	private bool playerSpawnQueue = false;
	public bool readyToStream = false;
	public override void _Ready()
	{	
		mPlayers = new MPlayer[maxPlayerCount];
		UI = GetParent<Node>().GetNode<ConnectScreenUI>("Player/ConnectScreenUI");
		player = GetParent<Node>().GetNode<Player>("Player");
		enemies = new BaseEnemy[50];
	}
	public async void connectClient(string adr){
		connection = new TCPConnection(false, this);
		await connection.Connect(adr);
		spawnHost();
	}
	public void hostTransmitEnemyState(int id){

	}
	public void addEnemyToTrackedMultiplayerEnemies(BaseEnemy newEnemy, int id){
		enemies[id] = newEnemy;
		enemyCount++;
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

				connection.acceptGenericTCPSignal();
				
				
				clientSend.sendData(packet,clientSend.RemoteIpEndPoint);
				clientSend.flushUDPPacket();
				RecievedDataStruct returnPacket = new RecievedDataStruct();
				playerHitPacket returnHitPacket = new playerHitPacket();
				enemyMovePacket enemyMove = new enemyMovePacket();
				await hostRecieve.RecieveData();
				var splitDataPacket = hostRecieve.getPresplitPacket();
				
			foreach(var semiSplitData in splitDataPacket){
				string [] splitData = semiSplitData.Split("/");
				if(splitData[0][0].ToString() == "d"){
					//damage calc
					GD.Print("Hit!");
					playerHitPacket hitPacket = new playerHitPacket();
					hitPacket.attackerID = splitData[1].ToInt();
					hitPacket.recieverID = splitData[2].ToInt();
					hitPacket.damage = splitData[3].ToInt();

					if(hitPacket.recieverID == player.id){
						player.takeDamage(hitPacket);
					}
				}
				else if(splitData[0][0].ToString() == "m"){
					//movement
					RecievedDataStruct clientPacket = new RecievedDataStruct();
					clientPacket.clientNumber = splitData[0][1].ToString().ToInt();
					clientPacket.anim = splitData[1][0];
					clientPacket.px = splitData[2];
					clientPacket.py = splitData[3];
					clientPacket.pz = splitData[4];
					clientPacket.rotation = splitData[5];
					if(clientPacket.clientNumber > maxPlayerCount){
						host.setOrientation(clientPacket);
					}
				}
				else if(splitData[0][0].ToString() == "e"){
					//enemy movement
					enemyMovePacket EnemyMovePacket = new enemyMovePacket();
					
					EnemyMovePacket.enemyNumber = splitData[0][1].ToString().ToInt();
					//EnemyMovePacket.anim = splitData[1][0];
					EnemyMovePacket.px = splitData[1];
					EnemyMovePacket.py = splitData[2];
					EnemyMovePacket.pz = splitData[3];
					EnemyMovePacket.rotation = splitData[4];

					enemies[EnemyMovePacket.enemyNumber].Position = new Vector3(EnemyMovePacket.px.ToFloat(),EnemyMovePacket.py.ToFloat(),EnemyMovePacket.pz.ToFloat());

				}
			}

				/*
				if(hostRecieve.packetType == 'm'){
					returnPacket = hostRecieve.getMovePacket();
					if(returnPacket.clientNumber > maxPlayerCount){
					host.setOrientation(returnPacket);
					}
				}
				else if(hostRecieve.packetType == 'e'){
					GD.Print("enemyMove");
					enemyMove = hostRecieve.getEnemyMovePacket();
					enemies[enemyMove.enemyNumber].Position = new Vector3(enemyMove.px.ToFloat(),enemyMove.py.ToFloat(),enemyMove.pz.ToFloat());
				}
				else if(hostRecieve.packetType == 'd'){
					returnHitPacket = hostRecieve.getHitPacket();
					if(returnHitPacket.recieverID == player.id){
						player.takeDamage(returnHitPacket);
					}
				}*/
				
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
			connection.acceptGenericTCPSignal();
			//Fix this shit later
			//Get Orientation of all clients
			for(int x = 0; x<playerCount; x++){
				await mPlayers[x].recieveUDPPacket();

				var splitDataPacket = mPlayers[x].getPresplitPacket();

				foreach(var semiSplitData in splitDataPacket){
					string [] splitData = semiSplitData.Split("/");
					if(splitData[0][0].ToString() == "d"){
						//damage calc
						GD.Print("Hit!");
						playerHitPacket hitPacket = new playerHitPacket();
						hitPacket.attackerID = splitData[1].ToInt();
						hitPacket.recieverID = splitData[2].ToInt();
						hitPacket.damage = splitData[3].ToInt();

						if(hitPacket.recieverID == player.id){
							player.takeDamage(hitPacket);
						}
					}
					else if(splitData[0][0].ToString() == "m"){
						//movement
						RecievedDataStruct clientPacket = new RecievedDataStruct();
						clientPacket.clientNumber = splitData[0][1].ToString().ToInt();
						clientPacket.anim = splitData[1][0];
						clientPacket.px = splitData[2];
						clientPacket.py = splitData[3];
						clientPacket.pz = splitData[4];
						clientPacket.rotation = splitData[5];
						mPlayers[x].setOrientation(clientPacket);
						if(clientPacket.clientNumber > maxPlayerCount){
							host.setOrientation(clientPacket);
						}
					}
				}
				
				/*
				char packetType = mPlayers[x].getPacketType();
				if(packetType == 'm'){
					mPlayers[x].recieveOrientation();
				}
				else if(packetType == 'd'){
					mPlayers[x].recieveDamage();
				}*/
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
				for(int y = maxPlayerCount+2; y < enemyCount+maxPlayerCount+2; y++){
					enemyMovePacket packett = new enemyMovePacket();
					packett.enemyNumber=enemies[y].id;
					packett.px = enemies[y].Position.X.ToString();
					packett.py = enemies[y].Position.Y.ToString();
					packett.pz = enemies[y].Position.Z.ToString();
					packett.rotation = enemies[y].Rotation.ToString();
					mPlayers[x].hostTransmitPositionToPlayers(packett);
				}
				mPlayers[x].flushUDPPacket();
			}
			
			
		}

	}

	public void broadcastRespawn(int id, Vector3 loc){
		playerLifePacket pkt = new playerLifePacket();
		pkt.playerID = id;
		pkt.px = loc.X.ToString();
		pkt.py = loc.Y.ToString();
		pkt.pz = loc.Z.ToString();
		if(!hosting){
			connection.clientSendRespawn(loc,id);
			return;
		}
		connection.serverSendClientRespawn(pkt);
	}
	public void playerDied(playerLifePacket pkt){
		Vector3 rotation = new Vector3(pkt.px.ToFloat(),pkt.py.ToFloat(),pkt.pz.ToFloat());
		
		if(pkt.playerID == maxPlayerCount+1){
			host.die(rotation);
		}
		else if(pkt.playerID == player.id){
			GD.Print("IM DYING!");
			player.die(rotation);
		}
		else{
			mPlayers[pkt.playerID].die(rotation);
		}
	}
	public void playerRespawn(playerLifePacket pkt){
		Vector3 location = new Vector3(pkt.px.ToFloat(),pkt.py.ToFloat(),pkt.pz.ToFloat());

		if(pkt.playerID == maxPlayerCount+1){
			host.respawn(location);
		}
		else if(pkt.playerID == player.id){
			GD.Print("IM Livin!");
			player.respawn(location);
		}
		else{
			mPlayers[pkt.playerID].respawn(location);
		}
		if(hosting){
			for(int x = 0; x < playerCount; x++){
				if(x == pkt.playerID){
					continue;
				}
				connection.broadcastPlayerRespawn(pkt);
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
