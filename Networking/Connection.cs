using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;

public class TCPConnection{
    
    TcpClient client;
    TcpListener listener;
    MultiplayerManager mpm;
    byte[] recieved;
    int defaultPort = 11001;

    bool isServer = false;

    public TCPConnection(bool setIsServer, in MultiplayerManager _mpm){
        isServer = setIsServer;
        mpm = _mpm;
    }
    public void Connect(string address){
        if(isServer){
            System.Net.IPAddress localAddr = System.Net.IPAddress.Parse("127.0.0.1");
            listener = new TcpListener(System.Net.IPAddress.Any,defaultPort);
            listener.BeginAcceptSocket(new System.AsyncCallback(acceptSocketCallbackServ), listener);
        }
        else{
            client = new TcpClient();
            client.Connect(address,11001);
            //client.BeginConnect(address,11001,new System.AsyncCallback(acceptSocketCallbackCli),client);
        }
    }
    public void onConnection(){
        mpm.playerConnect();
    }
    private void acceptSocketCallbackServ(System.IAsyncResult result){
        Godot.GD.Print("Hosting");
        Socket clientSocket = listener.EndAcceptSocket(result);
        int k = clientSocket.Receive(recieved);
        for (int i=0;i<k;i++){
            Godot.GD.Print(System.Convert.ToChar(recieved[i]));
        }
            
    }
    private void acceptSocketCallbackCli(System.IAsyncResult result){
        //Socket clientSocket = client.
        Godot.GD.Print("Clienttt");
    }

}