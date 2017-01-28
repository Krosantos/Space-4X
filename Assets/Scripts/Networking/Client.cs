#region

using System.Collections.Generic;
using Assets.Scripts.MapGen;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace Assets.Scripts.Networking
{
    public static class ClientExtensions
    {
        public static void Register(this NetworkClient client, bool connect)
        {
            Player.Me = new Player { Client = client };
            GameState.Me = new GameState();
            client.RegisterHandler(Messages.ChangeDiploStatus, temp);
            client.RegisterHandler(Messages.CreateUnit, OnCreateUnit);
            client.RegisterHandler(Messages.DestroyUnit, temp);
            client.RegisterHandler(Messages.EndGame, temp);
            client.RegisterHandler(Messages.EndTurn, temp);
            client.RegisterHandler(Messages.TransmitMap, OnMapTransmitted);
            client.RegisterHandler(Messages.MoveUnit, OnUnitMoveOrder);
            client.RegisterHandler(Messages.SendMessage, temp);
            client.RegisterHandler(Messages.TakeTurn, OnTurn);
            client.RegisterHandler(Messages.UnlockTech, temp);
            client.RegisterHandler(Messages.UpdateResources, temp);
            client.RegisterHandler(Messages.RegisterNewPlayer, OnRegistered);
            client.RegisterHandler(Messages.CheckForMap, OnMapCheck);
            if(connect)Player.Me.Client.Connect("localhost", 7777);
        }

        //Me is just for while I'm setting up handlers.
        private static void temp(NetworkMessage netMsg)
        {
        }

        private static void OnTurn(NetworkMessage netMsg)
        {
            Player.Me.OnTurn();
        }

        private static void OnCreateUnit(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<CreateUnitMsg>();
            var newUnitObject = Singleton<MonoBehaviour>.Instantiate(Unit.BaseUnit,
                GameState.Me.AllTiles[msg.TileId].transform.position, Quaternion.identity) as GameObject;
            var newUnit = newUnitObject.GetComponent<Unit>();
            newUnit.CurrentHealth = msg.MaxHealth;
            newUnit.UnitId = msg.UnitId;
            newUnit.MaxHealth = msg.MaxHealth;
            newUnit.MaxMoves = msg.MaxMoves;
            newUnit.MovesLeft = msg.MaxMoves;
            newUnit.CurrentTile = GameState.Me.AllTiles[msg.TileId];
            GameState.Me.AllTiles[msg.TileId].OccupyUnit = newUnit;
            GameState.Me.AllUnits.Add(newUnit.UnitId, newUnit);
            newUnit.Sprite = Resources.Load<Sprite>(msg.Sprite);
            newUnit.CreateMoveCostDictFromArray(msg.MoveCost);
            Player.Me.Turnables.Add(newUnit);
        }

        private static void OnMapCheck(NetworkMessage netMsg)
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
                mapGen.Launch(setting, () => { Singleton<MonoBehaviour>.Instance.StartCoroutine(SendChunksToServer(Player.Me.Client)); });
                
            }
            else if (msg.MapExists == MapState.Requested)
            {
                //The server puts you on a list, just chill for a second.
            }
            //Otherwise, it must exist, so ask for it.
            else Player.Me.Client.Send(Messages.TransmitMap, new TransmitMapMsg(true));
        }

        private static void OnMapTransmitted(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<TransmitMapMsg>();
            if (msg.IsRequest) Singleton<MonoBehaviour>.Instance.StartCoroutine(SendChunksToServer(Player.Me.Client));
            else
            {
                if (GameState.Me.MapLoader.AddPacketToMap(msg, GameState.Me)) GameState.Me.MapLoader.MakeClientMapFromArray();
            }

        }

        private static void OnRegistered(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<RegisterNewPlayerMsg>();
            Player.Me.Id = msg.PlayerId;
            Debug.Log("I have been registered! I am playerID:" + Player.Me.Id);
            
            Player.Me.Client.Send(Messages.CheckForMap, new CheckMapMsg());
        }

        private static void OnUnitMoveOrder(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<MoveUnitMsg>();
            Debug.Log("Move order received for unit "+msg.UnitId);
            var unit = GameState.Me.AllUnits[msg.UnitId];
            var tile = GameState.Me.AllTiles[msg.HexTileId];

            unit.Move(tile,msg.TotalMoveCost);
            //GameState.AllUnits[msg.UnitId].Move(GameState.AllTiles[msg.HexTileId],msg.TotalMoveCost);
        }

        static IEnumerator<WaitForSeconds> SendChunksToServer(NetworkClient client)
        {
            Debug.Log("Map made! Sending it to the server.");
            var wholeMap = GameState.Me.HexMap.SerializeMap();
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