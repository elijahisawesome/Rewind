using System;

public struct RecievedDataStruct{
    public char type;

    public char anim;
    public int clientNumber;
    //public string position;
    public string px;
    public string py;
    public string pz;
    public string rotation;
    
}
public struct playerHitPacket{
    public char type;
    public int recieverID;
    public int attackerID;
    public int damage;
    
}
public struct joinPacket{
    public int playerID;
    public int port;
    public int hostUDPPort;
    public string playerName;
}

public struct rewindingPacket{
    public bool currentlyRewinding;
}