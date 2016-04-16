using System.Collections.Generic;
using Assets.Scripts.MapGen;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class Server : NetworkBehaviour
    {
        public GameState GameState;

        private List<int> _clientsWaitingForMap;

        [UsedImplicitly]
        void Awake() {
            GameState = new GameState();
            NetworkServer.Listen(7777);
            NetworkServer.RegisterHandler(Messages.ChangeDiploStatus, temp);
            NetworkServer.RegisterHandler(Messages.CreateUnit, temp);
            NetworkServer.RegisterHandler(Messages.DestroyUnit, temp);
            NetworkServer.RegisterHandler(Messages.EndGame, temp);
            NetworkServer.RegisterHandler(Messages.EndTurn, temp);
            NetworkServer.RegisterHandler(Messages.TransmitMap, TransmitMap);
            NetworkServer.RegisterHandler(Messages.MoveUnit, temp);
            NetworkServer.RegisterHandler(Messages.SendMessage, temp);
            NetworkServer.RegisterHandler(Messages.TakeTurn, temp);
            NetworkServer.RegisterHandler(Messages.UnlockTech, temp);
            NetworkServer.RegisterHandler(Messages.UpdateResources, temp);
            NetworkServer.RegisterHandler(Messages.CheckForMap, CheckForMap );
            NetworkServer.RegisterHandler(MsgType.Connect, RegisterPlayerOnConnect);
        }

        //This is just for while I'm setting up handlers.
        private void temp(NetworkMessage netMsg) { }

        private void RegisterPlayerOnConnect(NetworkMessage netMsg)
        {
            var playerId = GameState.GenerateId();
            NetworkServer.SendToClient(netMsg.conn.connectionId,Messages.RegisterNewPlayer,new RegisterNewPlayerMsg(playerId));
            GameState.AllPlayers.Add(playerId);
        }

        private void TransmitMap(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<TransmitMapMsg>();
            if (msg.IsRequest) StartCoroutine(SendChunks(netMsg));
            else
            {
                if (GameState.MapLoader.AddPacketToMap(msg, GameState))
                {
                    GameState.AllTiles = GameState.MapLoader.MakeServerMapFromArray();
                    GameState.MapState = MapState.Complete;
                }
            }
        }

        private void CheckForMap(NetworkMessage netMsg)
        {
            Debug.Log("Client requested map state. Response: "+GameState.MapState);
            if (GameState.MapState == MapState.None)
            {
                //Tell the client to make a map
                NetworkServer.SendToClient(netMsg.conn.connectionId,Messages.CheckForMap,new CheckMapMsg(MapState.None));
                GameState.MapState = MapState.Requested;
            }
            else if (GameState.MapState == MapState.Requested)
            {
                //If we have a map brewing on a client, put this requester on a waiting list.
                if (_clientsWaitingForMap == null) _clientsWaitingForMap = new List<int>();
                _clientsWaitingForMap.Add(netMsg.conn.connectionId);
            }
            else
            {
                //If the map's already live, send it.
                StartCoroutine(SendChunks(netMsg));
            }

        }

        //Making this a coroutine stops the game from bottlenecking in the event the map's huuuge.
        private IEnumerator<WaitForSeconds> SendChunks(NetworkMessage netMsg)
        {
            var wholeMap = GameState.SerializedMap;
            for (int x = 0; x < wholeMap.Length / 500 + 1; x++)
            {
                var chunk = new int[500];
                for (int y = 0; y < 500; y++)
                {
                    if (x * 500 + y < wholeMap.Length)
                    {
                        chunk[y] = wholeMap[x * 500 + y];
                    }
                }
                NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.TransmitMap, new TransmitMapMsg(chunk, false));
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Telling the client that we sent the entire map.");
            NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.TransmitMap, new TransmitMapMsg(null, true));
        }
    }
}