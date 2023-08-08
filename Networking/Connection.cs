using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;

public class TCPConnection{
    
    TcpClient client;
    TcpClient clientFromListener;
    TcpListener listener;
    MultiplayerManager mpm;
    byte[] recieved;
    int defaultPort = 11002;

    bool isServer = false;

    public TCPConnection(bool setIsServer, in MultiplayerManager _mpm){
        isServer = setIsServer;
        mpm = _mpm;
    }
    public async System.Threading.Tasks.Task Connect(string address){
        if(isServer){
            Godot.GD.Print("Fuck!");
            System.Net.IPAddress localAddr = System.Net.IPAddress.Parse("127.0.0.1");
            listener = new TcpListener(System.Net.IPAddress.Any,defaultPort);
            listener.Start();
            listener.BeginAcceptTcpClient(new System.AsyncCallback(acceptSocketCallbackServ), listener);
        }
        else{
            client = new TcpClient();
            //client.Connect(address,defaultPort);
            
            client.BeginConnect(address,11002,new System.AsyncCallback(acceptSocketCallbackCli),client);

        }
    }

    private void acceptSocketCallbackServ(System.IAsyncResult result){
        Godot.GD.Print("Hosting from callback");
        
        clientFromListener = listener.EndAcceptTcpClient(result);
        //NetworkStream stream = clientFromListener.GetStream();
        System.Net.IPEndPoint endPoint = clientFromListener.Client.RemoteEndPoint as System.Net.IPEndPoint;
        
        mpm.playerConnect(endPoint.Address.ToString());
        /*
        if(clientFromListener.ReceiveBufferSize > 0){
             recieved = new byte[clientFromListener.ReceiveBufferSize];
             stream.Read(recieved, 0, clientFromListener.ReceiveBufferSize);             
             string msg = System.Text.Encoding.ASCII.GetString(recieved); //the message incoming
             Godot.GD.Print(msg);
         }*/

         
        /*
        for (int i=0;i<stream.Length;i++){
            Godot.GD.Print(System.Convert.ToChar(recieved[i]));
        }*/
            
    }
    private async void acceptSocketCallbackCli(System.IAsyncResult result){
        //Socket clientSocket = client.
        Godot.GD.Print("Clienttt");
        /*string testString = "hello!";
            System.Text.ASCIIEncoding asen= new System.Text.ASCIIEncoding();
            byte[] ba=asen.GetBytes(testString);
            NetworkStream stm = client.GetStream();
            stm.Write(ba,0,ba.Length);
            */
        client.EndConnect(result);
        NetworkStream stm = client.GetStream();
        string strr = "";
        if(client.ReceiveBufferSize > 0){
             recieved = new byte[client.ReceiveBufferSize];
             stm.Read(recieved, 0, client.ReceiveBufferSize);             
             strr = System.Text.Encoding.ASCII.GetString(recieved); //the message incoming
             Godot.GD.Print(strr);
             Godot.GD.Print('\n');
             stm.Flush();
         }
        determineNumberOfPlayersAndMyPortClient(strr);

        
    }
    private void determineNumberOfPlayersAndMyPortClient(string info){
        //this is a really bad way to do this, don't wanna think about structure rn tho.

        string[] words = info.Split('/');
        bool firstPort = false;
        bool secondPort = false;
        int x =0;
        foreach (var word in words)
        {
            if(word == "J"){
                x++;
                continue;
            }
            if(word.Length > 4 && !firstPort){
                mpm.setClientUDP(System.Convert.ToInt32(word));
                firstPort = true;
                x++;
                continue;
            }
            if (word.Length > 4 && !secondPort){
                //mpm.ff;
                mpm.setClientUDPHost(System.Convert.ToInt32(word));
                secondPort = true;
                x++;
                continue;
            }
            

            return;

        }

    }
    public void broadcastNewPlayerConnect(joinPacket newPlayer){

    }
    public void sendNewPlayerPortAndID(MPlayer newPlayer, Godot.Vector3 hostPos){
        joinPacket pkt = new joinPacket();
        pkt.playerID=newPlayer.id;
        pkt.port = newPlayer.port;
        pkt.hostUDPPort = mpm.getHostUDPPort();
        string spacer = "/";
        string strr = "J";
        strr+=spacer;
        strr+=pkt.port;
        strr+=spacer;
        strr+=pkt.hostUDPPort;
        strr+=spacer;
        strr+=pkt.playerID;
        strr+=spacer;
        strr+=hostPos.X.ToString() + "/"+ hostPos.Y.ToString() +"/"+ hostPos.Z.ToString();
        
        System.Text.ASCIIEncoding asen= new System.Text.ASCIIEncoding();
        byte[] ba=asen.GetBytes(strr);
        NetworkStream stm = clientFromListener.GetStream();
        stm.Write(ba,0,ba.Length);
        stm.Flush();
    }
}