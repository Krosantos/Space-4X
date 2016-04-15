using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Networking
{
    public class Server : NetworkBehaviour
    {
        Dictionary<NetworkConnection, int> _connectedPlayers;

        [UsedImplicitly]
        void Awake() {
            _connectedPlayers = new Dictionary<NetworkConnection, int>();
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
        }

        private void MapRequest(NetworkMessage netMsg)
        {
            Debug.Log("Map Request In!!");

            //Making this a coroutine stops the game from bottlenecking in the event the map's huuuge.
            StartCoroutine(SendChunks(netMsg));
        }

        IEnumerator<WaitForSeconds> SendChunks(NetworkMessage netMsg) {
            var wholeMap = HexTile.ParentMap.SerializeMap();
            for (int x = 0; x < wholeMap.Length / 500 + 1; x++)
            {
                Debug.Log("Sending chunk " + (x) + " of " + wholeMap.Length / 500 + "!");
                var chunk = new int[500];
                for (int y = 0; y < 500; y++)
                {
                    if (x * 500 + y < wholeMap.Length)
                    {
                        chunk[y] = wholeMap[x * 500 + y];
                    }
                }
                NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.LoadMap, new LoadMapMsg(chunk, false));
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Telling the client that we sent the entire map.");
            NetworkServer.SendToClient(netMsg.conn.connectionId, Messages.LoadMap, new LoadMapMsg(null, true));
        }
    }
}