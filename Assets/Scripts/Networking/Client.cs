#region

using Assets.Networking;
using Assets.Scripts.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#endregion

public class Client : NetworkClient
{
    public Dictionary<int, HexTile> AllTiles;
    public Dictionary<int, Unit> AllUnits;
    public GameState GameState;
    public Player Player;
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
        RegisterHandler(Messages.TransmitMap, OnMapTransmitted);
        RegisterHandler(Messages.MoveUnit, temp);
        RegisterHandler(Messages.SendMessage, temp);
        RegisterHandler(Messages.TakeTurn, OnTurn);
        RegisterHandler(Messages.UnlockTech, temp);
        RegisterHandler(Messages.UpdateResources, temp);
        RegisterHandler(Messages.RegisterNewPlayer, OnRegistered);
        RegisterHandler(Messages.CheckForMap, temp);
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

    private void OnMapCheck(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<CheckMapMsg>();
        if (msg.MapExists == MapState.None)
        {
            //Create a map
            //Tell the server you have a map
        }
        else if(msg.MapExists == MapState.Requested)
        {
            //Tell the server to put you on the list of people to alert once the map's done.
        }
        //Otherwise, it must exist, so ask for it.
        else Send(Messages.TransmitMap, new TransmitMapMsg());
    }

    private void OnMapTransmitted(NetworkMessage netMsg)
    {
        Debug.Log("We found a piece of the map!");
        var msg = netMsg.ReadMessage<TransmitMapMsg>();
        MapLoader.AddPacketToMap(msg);
    }

    private void OnRegistered(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<RegisterNewPlayerMsg>();
        Player.Id = msg.PlayerId;
        UiSelect.PlayerId = Player.Id;
        Debug.Log("I have been registered! I am playerID:" + Player.Id);

        //Change this to a map check!
        Send(Messages.TransmitMap, new TransmitMapMsg());
    }

    private void OnUnitMoveOrder(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MoveUnitMsg>();
        AllUnits[msg.UnitId].Move(AllTiles[msg.HexTileId]);
    }
}