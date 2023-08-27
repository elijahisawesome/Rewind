using System;
using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using Godot;

public class UDPRecieve{
    byte[] recieved;
    RecievedDataStruct clientPacket = new RecievedDataStruct();
    playerHitPacket hitPacket = new playerHitPacket();

    public char packetType = '\0';

    public System.Net.IPEndPoint LocalIpEndPoint;


    UdpClient udpClient;

    public UDPRecieve(ref UdpClient client){
        udpClient = client;
    }
    public async System.Threading.Tasks.Task RecieveData(){
        try{
            udpClient.BeginReceive(new System.AsyncCallback(recv),null);
            
        }
        catch(SocketException e){

        }
        
    }
    public void Connect(string ip, int port){
        LocalIpEndPoint = new  System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
        udpClient.Client.Bind(LocalIpEndPoint);
    }
    private void recv(System.IAsyncResult result){
        try{
            recieved = udpClient.EndReceive(result, ref LocalIpEndPoint);
            processPacket(); 
        }
        catch(SocketException e){
            GD.Print(e);
        }

    }
    private void processPacket(){
        try{
        string data = System.Text.Encoding.ASCII.GetString(recieved);
        string [] splitData = data.Split("/");
        if(splitData[0][0].ToString() == "d"){
            //damage calc
            GD.Print("Hit!");
            packetType = 'd';
            hitPacket.attackerID = splitData[1].ToInt();
            hitPacket.recieverID = splitData[2].ToInt();
            hitPacket.damage = splitData[3].ToInt();
        }
        else if(splitData[0][0].ToString() == "m"){
            //movement
            packetType = 'm';
            clientPacket.clientNumber = splitData[0][1].ToString().ToInt();
            clientPacket.anim = splitData[1][0];
            clientPacket.px = splitData[2];
            clientPacket.py = splitData[3];
            clientPacket.pz = splitData[4];
            clientPacket.rotation = splitData[5];
        }
        }
        catch(Exception e){

        }

    }
    public RecievedDataStruct getMovePacket(){
        return clientPacket;
    }
    public playerHitPacket getHitPacket(){
        return hitPacket;
    }
    public void resetPacketType(){
        packetType = '\0';
    }
    public void clearBuffer(){
        
    }
}