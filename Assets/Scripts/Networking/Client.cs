#region

using System.Collections.Generic;
using Assets.Scripts.MapGen;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace Assets.Scripts.Networking
{
    public class Client : NetworkClient
    {
        public GameState GameState;
        public Player Player;

        public void Awake()
        {
            Player = new Player {Client = this};
            GameState = new GameState();
            RegisterHandler(Messages.ChangeDiploStatus, temp);
            RegisterHandler(Messages.CreateUnit, temp);
            RegisterHandler(Messages.DestroyUnit, temp);
            RegisterHandler(Messages.EndGame, temp);
            RegisterHandler(Messages.EndTurn, temp);
            RegisterHandler(Messages.TransmitMap, OnMapTransmitted);
            RegisterHandler(Messages.MoveUnit, OnUnitMoveOrder);
            RegisterHandler(Messages.SendMessage, temp);
            RegisterHandler(Messages.TakeTurn, OnTurn);
            RegisterHandler(Messages.UnlockTech, temp);
            RegisterHandler(Messages.UpdateResources, temp);
            RegisterHandler(Messages.RegisterNewPlayer, OnRegistered);
            RegisterHandler(Messages.CheckForMap, OnMapCheck);
            Connect("localhost",7777);
        }

        //This is just for while I'm setting up handlers.
        private void temp(NetworkMessage netMsg)
        {
        }

        private void OnTurn(NetworkMessage netMsg)
        {
            Player.OnTurn();
        }

        private void OnMapCheck(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<CheckMapMsg>();
            if (msg.MapExists == MapState.None)
            {
                Debug.Log("Server wants me to make a map.");
                var mapGen = new MapGen.MapGen();
                var setting = new MapSetting
                {
                    AsteroidScore = 1,
                    IonScore = 1,
                    MixedScore = 2,
                    PlayerCount = 4,
                    Spiral = true,
                    XZones = 4,
                    YZones = 4,
                    RichnessScore = 0
                };
                mapGen.Launch(setting, () => { Singleton<MonoBehaviour>.Instance.StartCoroutine(SendChunksToServer(this)); });
                
            }
            else if (msg.MapExists == MapState.Requested)
            {
                //The server puts you on a list, just chill for a second.
            }
            //Otherwise, it must exist, so ask for it.
            else Send(Messages.TransmitMap, new TransmitMapMsg(true));
        }

        private void OnMapTransmitted(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<TransmitMapMsg>();
            if (msg.IsRequest) Singleton<MonoBehaviour>.Instance.StartCoroutine(SendChunksToServer(this));
            else
            {
                Debug.Log("We found a piece of the map!");
                if (GameState.MapLoader.AddPacketToMap(msg, GameState)) GameState.MapLoader.MakeClientMapFromArray();
            }

        }

        private void OnRegistered(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<RegisterNewPlayerMsg>();
            Player.Id = msg.PlayerId;
            Debug.Log("I have been registered! I am playerID:" + Player.Id);
            
            Send(Messages.CheckForMap, new CheckMapMsg());
        }

        private void OnUnitMoveOrder(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<MoveUnitMsg>();
            GameState.AllUnits[msg.UnitId].Move(GameState.AllTiles[msg.HexTileId],msg.TotalMoveCost);
        }

        IEnumerator<WaitForSeconds> SendChunksToServer(Client client)
        {
            Debug.Log("Map made! Sending it to the server.");
            var wholeMap = HexTile.ParentMap.SerializeMap();
            for (int x = 0; x < wholeMap.Length/500 + 1; x++)
            {
                var chunk = new int[500];
                for (int y = 0; y < 500; y++)
                {
                    if (x*500 + y < wholeMap.Length)
                    {
                        chunk[y] = wholeMap[x*500 + y];
                    }
                }
                client.Send(Messages.TransmitMap, new TransmitMapMsg(x,chunk, false));
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Telling the server that we sent the entire map.");
            client.Send(Messages.TransmitMap, new TransmitMapMsg(0,null, true));
        }
    }
}