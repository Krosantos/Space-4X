#region

using System.Collections.Generic;
using Assets.Scripts.MapGen;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace Assets.Scripts.Networking
{
    public class Client : NetworkClient
    {
        public GameState GameState;
        public Player Player;
        public List<ITurnable> Turnables;

        public void Awake()
        {
            GameState = new GameState();
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
            else if (msg.MapExists == MapState.Requested)
            {
                //The server puts you on a list, just chill for a second.
            }
            //Otherwise, it must exist, so ask for it.
            else Send(Messages.TransmitMap, new TransmitMapMsg(true));
        }

        private void OnMapTransmitted(NetworkMessage netMsg)
        {
            Debug.Log("We found a piece of the map!");
            var msg = netMsg.ReadMessage<TransmitMapMsg>();
            if (msg.IsRequest) Player.CheatCoroutine(SendChunksToServer(this));
            else
            {
                if(GameState.MapLoader.AddPacketToMap(msg, GameState)) GameState.MapLoader.MakeClientMapFromArray();
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
            GameState.AllUnits[msg.UnitId].Move(GameState.AllTiles[msg.HexTileId]);
        }

        IEnumerator<WaitForSeconds> SendChunksToServer(Client client)
        {
            var wholeMap = HexTile.ParentMap.SerializeMap();
            for (int x = 0; x < wholeMap.Length/500 + 1; x++)
            {
                Debug.Log("Sending chunk " + (x) + " of " + wholeMap.Length/500 + "!");
                var chunk = new int[500];
                for (int y = 0; y < 500; y++)
                {
                    if (x*500 + y < wholeMap.Length)
                    {
                        chunk[y] = wholeMap[x*500 + y];
                    }
                }
                client.Send(Messages.TransmitMap, new TransmitMapMsg(chunk, false));
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Telling the client that we sent the entire map.");
            client.Send(Messages.TransmitMap, new TransmitMapMsg(null, true));
        }
    }
}