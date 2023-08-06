using System;

public struct RecievedDataStruct{
    public int clientNumber;
    //public string position;
    public string px;
    public string py;
    public string pz;
    public string rotation;
}

public struct joinPacket{
    public int playerID;
    public int port;
    public int hostUDPPort;
    public string playerName;
}