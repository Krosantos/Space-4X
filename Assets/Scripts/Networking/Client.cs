#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#endregion

public class Client : NetworkClient
{
    public Dictionary<int, HexTile> AllTiles;
    public Dictionary<int, Unit> AllUnits;
    public int GameId, PlayerId;
    public List<ITurnable> Turnables;

    public void Awake()
    {
        AllUnits = new Dictionary<int, Unit>();
        AllTiles = new Dictionary<int, HexTile>();
        RegisterHandler(Messages.ChangeDiploStatus, temp);
        RegisterHandler(Messages.CreateUnit, temp);
        RegisterHandler(Messages.DestroyUnit, temp);
        RegisterHandler(Messages.EndGame, temp);
        RegisterHandler(Messages.EndTurn, temp);
        RegisterHandler(Messages.LoadMap, OnLoadMap);
        RegisterHandler(Messages.MoveUnit, temp);
        RegisterHandler(Messages.SendMessage, temp);
        RegisterHandler(Messages.TakeTurn, OnTurn);
        RegisterHandler(Messages.UnlockTech, temp);
        RegisterHandler(Messages.UpdateResources, temp);
        RegisterHandler(Messages.RegisterNewPlayer, OnRegistered);
    }

    //This is just for while I'm setting up handlers.
    private void temp(NetworkMessage netMsg)
    {
    }

    private void OnTurn(NetworkMessage netMsg)
    {
        foreach (var turnable in Turnables)
        {
            turnable.OnTurn();
        }
    }

    private void OnLoadMap(NetworkMessage netMsg)
    {
        Debug.Log("We found a piece of the map!");
        var msg = netMsg.ReadMessage<LoadMapMsg>();
        MapLoader.Player = this;
        MapLoader.AddPacketToMap(msg);
    }

    private void OnRegistered(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<RegisterNewPlayerMsg>();
        PlayerId = msg.PlayerId;
        UiSelect.PlayerId = PlayerId;
        Debug.Log("I have been registered! I am playerID:" + PlayerId);

        //Now, request the map.
        Send(Messages.LoadMap, new LoadMapMsg());
    }

    private void OnUnitMoveOrder(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MoveUnitMsg>();
        AllUnits[msg.UnitId].Move(AllTiles[msg.HexTileId]);
    }
}