using System;

public struct RecievedDataStruct{
    public int clientNumber;
    public string position;
    public string rotation;
}

public struct joinPacket{
    public int playerID;
    public string playerName;
}