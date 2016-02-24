using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Server : NetworkBehaviour {

    Dictionary<NetworkConnection, int> _connectedPlayers;
    IdGenerator _playerIdGen;

    [UsedImplicitly]
    void Awake() {
        _connectedPlayers = new Dictionary<NetworkConnection, int>();
        _playerIdGen = new IdGenerator();
        NetworkServer.Listen(7777);
        NetworkServer.RegisterHandler(Messages.ChangeDiploStatus, temp);
        NetworkServer.RegisterHandler(Messages.CreateUnit, temp);
        NetworkServer.RegisterHandler(Messages.DestroyUnit, temp);
        NetworkServer.RegisterHandler(Messages.EndGame, temp);
        NetworkServer.RegisterHandler(Messages.EndTurn, temp);
        NetworkServer.RegisterHandler(Messages.LoadMap, MapRequest);
        NetworkServer.RegisterHandler(Messages.MoveUnit, temp);
        NetworkServer.RegisterHandler(Messages.SendMessage, temp);
        NetworkServer.RegisterHandler(Messages.TakeTurn, temp);
        NetworkServer.RegisterHandler(Messages.UnlockTech, temp);
        NetworkServer.RegisterHandler(Messages.UpdateResources, temp);
        NetworkServer.RegisterHandler(MsgType.Connect, RegisterPlayerOnConnect);
    }

    //This is just for while I'm setting up handlers.
    private void temp(NetworkMessage netMsg) { }

    private void RegisterPlayerOnConnect(NetworkMessage netMsg) {
        _connectedPlayers.Add(netMsg.conn,_playerIdGen.GenerateId());
        NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.RegisterNewPlayer, new RegisterNewPlayerMsg(_connectedPlayers[netMsg.conn]));
        Debug.Log("A player connected! We sent them a message registering them as playerID " + _connectedPlayers[netMsg.conn]);
    }

    private void MapRequest(NetworkMessage netMsg)
    {
        Debug.Log("Map Request In!!");
        var wholeMap = HexTile.ParentMap.SerializeMap();
        for (int x = 0; x < wholeMap.Length/500 + 1; x++)
        {
            Debug.Log("Sending chunk " + (x) + " of " + wholeMap.Length/500 + "!");
            var chunk = new int[500];
            for (int y = 0; y < 500; y++)
            {
                if (x * 500 + y < wholeMap.Length)
                {
                    chunk[y] = wholeMap[x * 500 + y];
                }
            }
            NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.LoadMap, new LoadMapMsg(chunk, false));
        }
        NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.LoadMap, new LoadMapMsg(null, true));
    }

}

public class IdGenerator {
    int _currentNumber;

    public IdGenerator() {
        _currentNumber = 0;
    }

    public int GenerateId() {
        _currentNumber++;
        return _currentNumber;
    }
}