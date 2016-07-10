using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using UnityEngine;
using Object = UnityEngine.Object;
using Terrain = Assets.Scripts.Utility.Terrain;

namespace Assets.Scripts.MapGen
{
    public class MapLoader
    {
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
    
        private Dictionary<int, int[]> _allMapPieces;
        private int[] _serializedMap;

        public bool AddPacketToMap(TransmitMapMsg msg, GameState gameState)
        {
            //Initialize the list if not present.
            if(_allMapPieces == null) _allMapPieces = new Dictionary<int,int[]>();

            //If it's the final chunk, create the serialized map.
            if (msg.IsFinalPiece)
            {
                Debug.Log("We have the entire map!");
                _serializedMap = new int[(_allMapPieces.Count+1)*500];
                for (var x = 0; x < _allMapPieces.Count; x++)
                {
                    for (var y = 0; y < _allMapPieces[x].Length; y++)
                    {
                        _serializedMap[x*500 + y] = _allMapPieces[x][y];
                    }
                }
                gameState.SerializedMap = _serializedMap;
                return true;
            }

            //Otherwise, Add the chunk of map to the list.
            Debug.Log("Adding this piece to the total map!");
            _allMapPieces.Add(msg.Index,msg.SerializedMapChunk);
            return false;
        }

        public Dictionary<int,HexTile> MakeServerMapFromArray()
        {
            var hexHolder = new GameObject {name = "Map"};
            var result = new Dictionary<int,HexTile>();
            var hexMap = new HexMap(_serializedMap[0], _serializedMap[1]);
            Debug.Log("The map we got from the client is "+_serializedMap[0]+" by "+_serializedMap[1]+" tiles big.");
            HexTile.ParentMap = hexMap;
            for (var t = 2; t < _serializedMap.Length; t += 8)
            {
                //Serialized maps have large deserts of 0,0,0,0,0... on their tails. Cut this off when that happens.
                if (_serializedMap[t + 2] == 0) break;

                var newObject = new GameObject();
                newObject.transform.parent = hexHolder.transform;
                var tile = newObject.AddComponent<HexTile>();
                tile.X = _serializedMap[t];
                tile.Y = _serializedMap[t + 1];
                tile.Id = _serializedMap[t + 2];
                tile.Terrain = (Terrain) _serializedMap[t + 3];
                tile.Resource = new Resource(_serializedMap[t + 4], _serializedMap[t + 5], _serializedMap[t + 6], _serializedMap[t + 7]);
                newObject.name = tile.X + ", " + tile.Y;

                //Add the tile to reference lists.
                try
                {
                    result.Add(tile.Id, tile);
                }
                catch (Exception e)
                {
                    Debug.Log(_serializedMap[t + 2]);
                    Debug.Log(e);
                }
            }
            Debug.Log("Built the entire map!");
            return result;
        }

        public void MakeClientMapFromArray()
        {
            var mapObject = new GameObject {name = "Map"};
            var hexMap = new HexMap(_serializedMap[0], _serializedMap[1]);
            HexTile.ParentMap = hexMap;

            for (var t = 2; t < _serializedMap.Length; t += 8)
            {
                //Serialized maps have large deserts of 0,0,0,0,0... on their tails. Cut this off when that happens.
                if (_serializedMap[t + 2] == 0) break;

                GameObject hexObject;
                if (_serializedMap[t] % 2 == 0)
                {
                    hexObject = Object.Instantiate(HexPrefab, new Vector3(_serializedMap[t] * 0.815f, _serializedMap[t+1], 0), Quaternion.identity) as GameObject;
                }
                else
                {
                    hexObject = Object.Instantiate(HexPrefab, new Vector3(_serializedMap[t] * 0.815f, _serializedMap[t+1] + 0.5f, 0), Quaternion.identity) as GameObject;
                }
                if (hexObject == null) continue;
                var tile = hexObject.GetComponent<HexTile>();
                hexMap.AllTiles[_serializedMap[t], _serializedMap[t+1]] = tile;
                tile.X = _serializedMap[t];
                tile.Y = _serializedMap[t+1];
                tile.Id = _serializedMap[t+2];
                tile.SetTerrain((Terrain)_serializedMap[t+3]);
                tile.Resource = new Resource(_serializedMap[t+4], _serializedMap[t+5], _serializedMap[t+6], _serializedMap[t+7]);
                hexObject.name = (tile.X + ", " + tile.Y);
                hexObject.transform.parent = mapObject.transform;

                //Add the tile to reference lists.
                hexMap.TileList.Add(tile);
            }
        }
    }
}
