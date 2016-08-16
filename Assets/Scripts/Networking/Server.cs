using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class Server : NetworkBehaviour
    {
        public GameState GameState;
        public Dictionary<int, bool> PlayersTakingTurn;
        private Dictionary<int, int> _connectionPlayerMapping;  
        private List<int> _clientsWaitingForMap;

        [UsedImplicitly]
        void Awake() {
            GameState = new GameState();
            PlayersTakingTurn = new Dictionary<int, bool>();
            _connectionPlayerMapping = new Dictionary<int, int>();
            _clientsWaitingForMap = new List<int>();
            NetworkServer.RegisterHandler(Messages.ChangeDiploStatus, temp);
            NetworkServer.RegisterHandler(Messages.CreateUnit, temp);
            NetworkServer.RegisterHandler(Messages.DestroyUnit, temp);
            NetworkServer.RegisterHandler(Messages.EndGame, temp);
            NetworkServer.RegisterHandler(Messages.EndTurn, EndTurn);
            NetworkServer.RegisterHandler(Messages.TransmitMap, TransmitMap);
            NetworkServer.RegisterHandler(Messages.MoveUnit, temp);
            NetworkServer.RegisterHandler(Messages.SendMessage, temp);
            NetworkServer.RegisterHandler(Messages.TakeTurn, temp);
            NetworkServer.RegisterHandler(Messages.UnlockTech, temp);
            NetworkServer.RegisterHandler(Messages.UpdateResources, temp);
            NetworkServer.RegisterHandler(Messages.CheckForMap, CheckForMap );
            NetworkServer.RegisterHandler(MsgType.Connect, RegisterPlayerOnConnect);
            NetworkServer.Listen(7777);
        }

        //This is just for while I'm setting up handlers.
        private void temp(NetworkMessage netMsg) { }

        private void RegisterPlayerOnConnect(NetworkMessage netMsg)
        {
            //Generate an Id for the player, and add it to the game state, the mapping of connections to players, and the turn dictionary.
            var playerId = GameState.GenerateId();
            NetworkServer.SendToClient(netMsg.conn.connectionId,Messages.RegisterNewPlayer,new RegisterNewPlayerMsg(playerId));
            GameState.AllPlayers.Add(playerId);
            PlayersTakingTurn.Add(playerId,false);
            _connectionPlayerMapping.Add(netMsg.conn.connectionId,playerId);
        }

        private void TransmitMap(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<TransmitMapMsg>();
            if (msg.IsRequest) StartCoroutine(SendChunks(netMsg.conn.connectionId));
            else
            {
                if (GameState.MapLoader.AddPacketToMap(msg, GameState))
                {
                    //The map is done!
                    GameState.AllTiles = GameState.MapLoader.MakeServerMapFromArray();
                    GameState.MapState = MapState.Complete;
                    foreach (var connId in _clientsWaitingForMap)
                    {
                        StartCoroutine(SendChunks(connId));
                    }
                }
            }
        }

        private void EndTurn(NetworkMessage netMsg)
        {
            PlayersTakingTurn[_connectionPlayerMapping[netMsg.conn.connectionId]] = false;
            //If anyone is still taking their turn, let 'em.
            if (PlayersTakingTurn.Any(x => x.Value)) return;

            //Otherwise, do stuff.
            foreach (var pair in PlayersTakingTurn)
            {
                PlayersTakingTurn[pair.Key]= true;
                NetworkServer.SendToAll(Messages.TakeTurn, new TakeTurnMsg());
            }
        }

        private void CheckForMap(NetworkMessage netMsg)
        {
            Debug.Log("Client requested map state. Response: "+GameState.MapState);
            switch (GameState.MapState)
            {
                case MapState.None:
                    //Tell the client to make a map
                    NetworkServer.SendToClient(netMsg.conn.connectionId,Messages.CheckForMap,new CheckMapMsg(MapState.None));
                    GameState.MapState = MapState.Requested;
                    break;
                case MapState.Requested:
                    //If we have a map brewing on a client, put this requester on a waiting list.
                    _clientsWaitingForMap.Add(netMsg.conn.connectionId);
                    break;
                default:
                    StartCoroutine(SendChunks(netMsg.conn.connectionId));
                    break;
            }
        }

        //Making this a coroutine stops the game from bottlenecking in the event the map's huuuge.
        private IEnumerator<WaitForSeconds> SendChunks(int connId)
        {
            var wholeMap = GameState.SerializedMap;
            Debug.Log("Sending the map! It's gonna be "+wholeMap[0]+" by "+wholeMap[1]+" tiles big!");
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
                NetworkServer.SendToClient(connId, Messages.TransmitMap, new TransmitMapMsg(x,chunk, false));
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Telling the client that we sent the entire map.");
            NetworkServer.SendToClient(connId, Messages.TransmitMap, new TransmitMapMsg(0,null, true));
        }
    }
}