using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using Godot;

public class UDPRecieve{
    byte[] recieved;
    RecievedDataStruct clientPacket = new RecievedDataStruct();
    playerHitPacket hitPacket = new playerHitPacket();

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
        LocalIpEndPoint = new  System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port);
        udpClient.Client.Bind(LocalIpEndPoint);
        //udpClient.Connect(ip,port);
    }
    private void recv(System.IAsyncResult result){
        recieved = udpClient.EndReceive(result, ref LocalIpEndPoint);
        //GD.Print("Byte recieved");
        processPacket(); 
    }
    private void processPacket(){
        string data = System.Text.Encoding.ASCII.GetString(recieved);
        string [] splitData = data.Split("/");

        if(splitData[0] == "d"){
            //damage calc
            clientPacket.clientNumber = splitData[1].ToInt();
            clientPacket.px = splitData[2];
            clientPacket.py = splitData[3];
            clientPacket.pz = splitData[4];
            clientPacket.rotation = splitData[5];
        }
        else if(splitData[0] == "m"){
            //movement
        }
        //GD.Print(data);


    }
    public RecievedDataStruct getPacket(){
        return clientPacket;
    }
    public void clearBuffer(){
        
    }
    public void sendPacket(){

    }


}