using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;


public class UDPSend{
    int x = 0;
    string testString = "Is?";
    byte[] sendBytesTest = System.Text.Encoding.ASCII.GetBytes("Is anybody there?");
    byte [] sendBytes;

    UdpClient udpClient;
    public UDPSend(ref UdpClient client){
        udpClient = client;
    }
    public bool Connect(string adr){
        bool connected = true;
        udpClient.Connect(adr, 11000);
        Godot.GD.Print(adr);

        return connected;
    }
    public void SendDataTest(){
        udpClient.Send(sendBytesTest);
    }
    public void iterate(){
        x++;
        string newTestString = x+testString;
        sendBytesTest = System.Text.Encoding.ASCII.GetBytes(newTestString);
    }
    public void sendData(RecievedDataStruct packet){
        string d ="";
        d+=packet.clientNumber;
        d+=packet.position;
        d+=packet.rotation;
        sendBytes = System.Text.Encoding.ASCII.GetBytes(d);

        udpClient.Send(sendBytes);
    }
}
