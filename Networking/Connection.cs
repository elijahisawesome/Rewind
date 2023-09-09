using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Encodings;
using System.Threading;
using System.Threading.Tasks;
using Godot;

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
            listener = new TcpListener(System.Net.IPAddress.Any,defaultPort);
            listener.Start();
            listener.BeginAcceptTcpClient(new System.AsyncCallback(acceptSocketCallbackServ), listener);
        }
        else{
            client = new TcpClient();
            client.BeginConnect(address,11001,new System.AsyncCallback(acceptSocketCallbackCli),client);
        }
    }

    private async void acceptSocketCallbackServ(System.IAsyncResult result){
        Godot.GD.Print("Hosting from callback");
        
        clientFromListener = listener.EndAcceptTcpClient(result);
        //NetworkStream stream = clientFromListener.GetStream();
        System.Net.IPEndPoint endPoint = clientFromListener.Client.RemoteEndPoint as System.Net.IPEndPoint;
        
        mpm.playerConnect(endPoint.Address.ToString());
            
    }
    private async void acceptSocketCallbackCli(System.IAsyncResult result){
        //Socket clientSocket = client.
        Godot.GD.Print("Clienttt");
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

    public void serverSendClientDeath(int id, Vector3 rotation){
        string deliminator = "/";
        string strr = "D";
        strr+=deliminator;
        strr+= id;
        strr+=deliminator;
        strr+=rotation.X;
        strr+=deliminator;
        strr+=rotation.Y;
        strr+=deliminator;
        strr+=rotation.Z;
        strr+=deliminator;
        
        System.Text.ASCIIEncoding asen= new System.Text.ASCIIEncoding();
        byte[] ba=asen.GetBytes(strr);
        NetworkStream stm = clientFromListener.GetStream();
        stm.Write(ba,0,ba.Length);
        stm.Flush();
    }
    
    public void clientSendRespawn(Vector3 location, int id){
        string deliminator = "/";
        string strr = "L";
        strr+=deliminator;
        strr+= id;
        strr+=deliminator;
        strr+=location.X;
        strr+=deliminator;
        strr+=location.Y;
        strr+=deliminator;
        strr+=location.Z;
        strr+=deliminator;
        System.Text.ASCIIEncoding asen= new System.Text.ASCIIEncoding();
        byte[] ba=asen.GetBytes(strr);
        NetworkStream stm = client.GetStream();
        stm.Write(ba,0,ba.Length);
        stm.Flush();
    }
    public async void serverSendClientRespawn(playerLifePacket pkt){
        string deliminator = "/";
        string strr = "L";
        strr+=deliminator;
        strr+= pkt.playerID;
        strr+=deliminator;
        strr+=pkt.px;
        strr+=deliminator;
        strr+=pkt.py;
        strr+=deliminator;
        strr+=pkt.pz;
        strr+=deliminator;

        System.Text.ASCIIEncoding asen= new System.Text.ASCIIEncoding();
        byte[] ba=asen.GetBytes(strr);
        NetworkStream stm = clientFromListener.GetStream();
        stm.Write(ba,0,ba.Length);
        stm.Flush();
    }
    public void processGenericTCPSignal(string info){
        GD.Print(info);
        playerLifePacket pkt = new playerLifePacket();
        string[] words = info.Split('/');
        if(words[0] == "D"){
            GD.Print(words[0]);
            GD.Print(words[1]);
            GD.Print(words[2]);
            GD.Print(words[3]);
            GD.Print(words[4]);
            pkt.Dying = 1;
            pkt.playerID = words[1].ToInt();
            pkt.px = words[2];
            pkt.py = words[3];
            pkt.pz = words[4];
            mpm.playerDied(pkt);
        }
        if(words[0] == "L"){
            GD.Print(words[0]);
            GD.Print(words[1]);
            GD.Print(words[2]);
            GD.Print(words[3]);
            GD.Print(words[4]);
            pkt.playerID = words[1].ToInt();
            pkt.px = words[2];
            pkt.py = words[3];
            pkt.pz = words[4];
            mpm.playerRespawn(pkt);
        }
    }
    public void broadcastPlayerRespawn(playerLifePacket pkt){

    }
    public async System.Threading.Tasks.Task acceptGenericTCPSignal(){
        //var p = client.Client.EndReceive();
        if(mpm.hosting){
            try{
                if(clientFromListener.ReceiveBufferSize > 0){
                    NetworkStream stm = clientFromListener.GetStream();
                    string strr = "";
                    int offset = 0;
                    //recieved = new byte[client.ReceiveBufferSize];
                    byte[]buffer = new byte[50];
                    int bytesRead = await stm.ReadAsync(buffer, offset, buffer.Length);
                        offset += bytesRead;
                        strr = System.Text.Encoding.ASCII.GetString(buffer); //the message incoming
                        Godot.GD.Print('\n');
                    GD.Print("Fuck!");
                    processGenericTCPSignal(strr);
                    Godot.GD.Print('\n');
                    stm.Flush();
                }
            }
            catch(Exception e){
                GD.Print(e);
            }
        }
        else if (!mpm.hosting){
            try{
                if(client.ReceiveBufferSize > 0){
                    NetworkStream stm = client.GetStream();
                    string strr = "";
                    int offset = 0;
                    recieved = new byte[client.ReceiveBufferSize];
                    byte[]buffer = new byte[50];
                    int bytesRead = await stm.ReadAsync(buffer, offset, buffer.Length - offset);
                        offset += bytesRead;
                        strr = System.Text.Encoding.ASCII.GetString(buffer); //the message incoming
                        Godot.GD.Print('\n');
                
                    processGenericTCPSignal(strr);
                    Godot.GD.Print('\n');
                    stm.Flush();
                }
            }
            catch{

            }
        }
        
        

         //return;
        //await determin
    }
    private void genRecv(System.IAsyncResult result){

    }
    public async void clientRecieveClientStatusFromServer(){

    }
    private void determineNumberOfPlayersAndMyPortClient(string info){
        //this is a really bad way to do this, don't wanna think about structure rn tho.
        client.GetStream().Flush();

        string[] words = info.Split('/');
        bool firstPort = false;
        bool secondPort = false;
        int x =0;
        GD.Print(words);
        foreach (var word in words)
        {
            if(word == "J"){
                x++;
                continue;
            }
            if(word[0] == 'I'){
                mpm.setPlayerAndMPMID(word[1].ToString().ToInt());
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
        strr+= "I";
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