using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using Godot;

public class UDPRecieve{
    byte[] recieved;
    RecievedDataStruct clientPacket = new RecievedDataStruct();

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
        //GD.Print(data);

        clientPacket.clientNumber = splitData[0].ToInt();
        clientPacket.px = splitData[1];
        clientPacket.py = splitData[2];
        clientPacket.pz = splitData[3];
        clientPacket.rotation = splitData[4];
    }
    public RecievedDataStruct getPacket(){
        return clientPacket;
    }
    public void clearBuffer(){
        
    }
    public void sendPacket(){

    }


}