using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using System.Net;
using Godot;

public class UDPSend{

    public IPEndPoint RemoteIpEndPoint;

    int x = 0;
    string sendString = "";
    byte[] sendBytesTest = System.Text.Encoding.ASCII.GetBytes("Is anybody there?");
    byte [] sendBytes;

    string hostAddress;

    UdpClient udpClient;
    public UDPSend(ref UdpClient client){
        udpClient = client;
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    }
    public bool Connect(string adr, int port){
        RemoteIpEndPoint =  new IPEndPoint(IPAddress.Parse(adr), port);

        return true;
    }
    public void SendDataTest(){
        udpClient.Send(sendBytesTest);
    }
    public void SendConnect(){
        sendBytes = System.Text.Encoding.ASCII.GetBytes("Connect, Hello!");
        udpClient.Send(sendBytes);
    }
    
    public void sendData(RecievedDataStruct packet, IPEndPoint rEP){
        string d ="m";
        string deliminator = "/";
        d+=packet.clientNumber;
        d+=deliminator;
        d+=packet.anim;
        d+=deliminator;
        d+=packet.px;
        d+=deliminator;
        d+=packet.py;
        d+=deliminator;
        d+=packet.pz;
        d+=deliminator;
        d+=packet.rotation;

        concatinatePackets(d,rEP);
    }
    public void sendData(playerHitPacket packet, IPEndPoint rEP){
        string d ="d";
        string deliminator = "/";
        d+=deliminator;
        d+=packet.attackerID;
        d+=deliminator;
        d+=packet.recieverID;
        d+=deliminator;
        d+=packet.damage;

        concatinatePackets(d,rEP);
    }
    public void sendData(enemyMovePacket packet, IPEndPoint rEP){
        string d ="e";
        string deliminator = "/";
        d+=packet.enemyNumber;
        d+=deliminator;
        d+=packet.px;
        d+=deliminator;
        d+=packet.py;
        d+=deliminator;
        d+=packet.pz;
        d+=deliminator;
        d+=packet.targetID;
        /*
        d+=packet.mx;
        d+=deliminator;
        d+=packet.mz;
        d+=packet.rotation;
        */
        concatinatePackets(d, rEP);
    }
    	private void concatinatePackets(string str, IPEndPoint rEP){
        sendString+=str;
        sendString+="+";
        
		
	}
    public void flushUDPPacket(){
        sendString = sendString.Remove(sendString.Length-1);
        sendBytes = System.Text.Encoding.ASCII.GetBytes(sendString);
        udpClient.Client.SendTo(sendBytes, RemoteIpEndPoint);
        sendString = "";
    }
}
