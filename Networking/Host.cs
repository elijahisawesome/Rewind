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
        //udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

    }
    public async System.Threading.Tasks.Task RecieveData(){
        try{
            //byte[] recieveBytes = await udpClient.ReceiveAsync(ref RemoteIpEndPoint);
            //returnData = System.Text.Encoding.ASCII.GetString(recieveBytes);
            udpClient.BeginReceive(new System.AsyncCallback(recv),null);
            
        }
        catch(SocketException e){

        }
        
    }
    public void Connect(string ip, int port){
        //GD.Print(ip);
        //GD.Print(port);
        LocalIpEndPoint = new  System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
        udpClient.Client.Bind(LocalIpEndPoint);
        //udpClient.Connect(ip,port);
    }
    private void recv(System.IAsyncResult result){
        try{
            recieved = udpClient.EndReceive(result, ref LocalIpEndPoint);
            //GD.Print("Byte recieved");
            processPacket(); 
        }
        catch(SocketException e){
            GD.Print(e);
        }

    }
    private void processPacket(){
        string data = System.Text.Encoding.ASCII.GetString(recieved);
        string [] splitData = data.Split("/");
        //GD.Print(splitData[1]);
        //GD.Print(splitData[2]);
        //GD.Print(splitData[3]);
        //GD.Print(splitData[4]);
        //GD.Print(data);
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
            //GD.Print(splitData[0]);
            packetType = 'm';
            clientPacket.clientNumber = splitData[0][1].ToString().ToInt();
            clientPacket.px = splitData[1];
            clientPacket.py = splitData[2];
            clientPacket.pz = splitData[3];
            clientPacket.rotation = splitData[4];
        }
        //GD.Print(data);


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
    public void sendPacket(){

    }


}