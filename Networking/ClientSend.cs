using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;


public class UDPSend{

    int x = 0;
    string testString = "Is?";
    byte[] sendBytesTest = System.Text.Encoding.ASCII.GetBytes("Is anybody there?");
    byte [] sendBytes;

    string hostAddress;

    UdpClient udpClient;
    public UDPSend(ref UdpClient client){
        udpClient = client;
    }
    public bool Connect(string adr, int port){

        udpClient.Connect(adr, port);
        //Godot.GD.Print(adr);

        return true;
    }
    public void SendDataTest(){
        udpClient.Send(sendBytesTest);
    }
    public void SendConnect(){
        sendBytes = System.Text.Encoding.ASCII.GetBytes("Connect, Hello!");
        udpClient.Send(sendBytes);
    }
    public void iterate(){
        x++;
        string newTestString = x+testString;
        sendBytesTest = System.Text.Encoding.ASCII.GetBytes(newTestString);
    }
    public void sendData(RecievedDataStruct packet){
        string d ="";
        string deliminator = "/";
        d+=packet.clientNumber;
        d+=deliminator;
        d+=packet.px;
        d+=deliminator;
        d+=packet.py;
        d+=deliminator;
        d+=packet.pz;
        d+=deliminator;
        d+=packet.rotation;
        Godot.GD.Print(d);
        sendBytes = System.Text.Encoding.ASCII.GetBytes(d);
    	
        udpClient.Send(sendBytes);
    }
}
