using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using Godot;

public class UDPRecieve{
    byte[] recieved;
    RecievedDataStruct clientPacket = new RecievedDataStruct();

    System.Net.IPEndPoint RemoteIpEndPoint = new  System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
    UdpClient udpClient = new UdpClient(11000);
    public async System.Threading.Tasks.Task RecieveData(){
        try{
            //byte[] recieveBytes = await udpClient.ReceiveAsync(ref RemoteIpEndPoint);
            //returnData = System.Text.Encoding.ASCII.GetString(recieveBytes);
            udpClient.BeginReceive(new System.AsyncCallback(recv),null);
            processPacket();
        }
        catch(SocketException e){

        }
        
    }
    private void recv(System.IAsyncResult result){
        recieved = udpClient.EndReceive(result, ref RemoteIpEndPoint);        
    }
    private void processPacket(){
        string data = System.Text.Encoding.ASCII.GetString(recieved);

        GD.Print(data);

        clientPacket.clientNumber = data[0];
        clientPacket.position = data[1].ToString();
        clientPacket.rotation = data[2].ToString();
    }
    public RecievedDataStruct getPacket(){
        return clientPacket;
    }
    public void clearBuffer(){
        
    }
    public void sendPacket(){

    }

}