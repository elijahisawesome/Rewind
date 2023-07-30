using Godot;
using System;
using System.Threading;

public partial class MultiplayerManager : Node
{
	Player localPlayer;
	MPlayer [] mPlayers;
	int playerCount = 0;
	int maxPlayerCount = 4;
	// Called when the node enters the scene tree for the first time.
		//Test Networking;
	////////////
	public UDPSend clientSend = new UDPSend();
	public UDPRecieve hostRecieve = new UDPRecieve();
	public ConnectScreenUI UI;

	public bool hosting;

	public override void _Ready()
	{
		mPlayers = new MPlayer[maxPlayerCount];
		UI = GetParent<Node>().GetNode<ConnectScreenUI>("Player/ConnectScreenUI");
	}
	public void connectClient(string adr){
		clientSend.Connect(adr);
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
			}
		}

	}

    //When connection is recieved, spawn new mPlayer;
	public void playerConnect(){
		if(playerCount < maxPlayerCount){
			mPlayers[playerCount] = new MPlayer();
			playerCount++;
		}
		if(hosting){
			broadcastConnect();
		}

	}
	public void setHosting(){
		hosting = true;
	}

	//Send all relavent info to all mPlayers;
	private void broadcast(){

	}
	private void broadcastConnect(){
		for(int x = 0; x<playerCount; x++){
				
		}
	}
}
