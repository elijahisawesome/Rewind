using Godot;
using System;
using System.Threading;
using System.Net.Sockets;

public partial class MultiplayerManager : Node
{
	Player localPlayer;
	MPlayer [] mPlayers;
	int playerCount = 0;
	int maxPlayerCount = 4;
	UdpClient udpClient = new UdpClient(11000);
	// Called when the node enters the scene tree for the first time.
		//Test Networking;
	////////////
	public UDPSend clientSend;
	public UDPRecieve hostRecieve;
	public ConnectScreenUI UI;
	TCPConnection connection;

	public bool hosting;

	public override void _Ready()
	{
		clientSend = new UDPSend(ref udpClient);
		hostRecieve = new UDPRecieve(ref udpClient);
		mPlayers = new MPlayer[maxPlayerCount];
		UI = GetParent<Node>().GetNode<ConnectScreenUI>("Player/ConnectScreenUI");
	}
	public void connectClient(string adr){
		connection = new TCPConnection(false, this);
		clientSend.Connect(adr, ref connection);
		//clientSend.SendConnect();
	}

		public override async void _PhysicsProcess(double delta)
	{
		if(UI.isConnected()){
			//Genericize
			clientSend.iterate();
			clientSend.SendDataTest();

			RecievedDataStruct packet = new RecievedDataStruct();
			await hostRecieve.RecieveData();
			packet = hostRecieve.getPacket();
		}
		else if(UI.isHosting()){
			try{
				RecievedDataStruct packet = new RecievedDataStruct();
				await hostRecieve.RecieveData();
				packet = hostRecieve.getPacket();
			}
			catch(Exception e){

			}

			//Fix this shit later
			/*
			for(int x = 0; x<playerCount; x++){
				await mPlayers[x].recieveOrientation();
				//mPlayers[x].setOrientation();
			}
			for(int x = 0; x<playerCount; x++){
				RecievedDataStruct packet = new RecievedDataStruct();
				packet.clientNumber=x;
				packet.position = mPlayers[x].Position.ToString();
				packet.rotation = mPlayers[x].Rotation.ToString();
				mPlayers[x].hostTransmitPositionToPlayers(packet);
			}*/
		}

	}

    //When connection is recieved, spawn new mPlayer;
	public void playerConnect(){
		GD.Print("Player COnnected");
		if(playerCount < maxPlayerCount){
			mPlayers[playerCount] = new MPlayer();
			GetTree().Root.AddChild(mPlayers[playerCount]);
			playerCount++;
		}
		if(hosting){
			broadcastConnect();
		}

	}
	public void setHosting(){
		hosting = true;
		connection = new TCPConnection(true, this);
	}

	//Send all relavent info to all mPlayers;
	private void broadcast(){

	}
	private void broadcastConnect(){
		for(int x = 0; x<playerCount; x++){
				
		}
	}
}
