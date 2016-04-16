using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.MapGen
{
    public class MapLoader
    {
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
    
        private List<int[]> _allMapPieces;
        private int[] _serializedMap;

        public bool AddPacketToMap(TransmitMapMsg msg, GameState gameState)
        {
            //Initialize the list if not present.
            if(_allMapPieces == null) _allMapPieces = new List<int[]>();

            //If it's the final chunk, create the serialized map.
            if (msg.IsFinalPiece)
            {
                Debug.Log("We have the entire map!");
                _serializedMap = new int[_allMapPieces.Sum(x => x.Length)];
                var index = 0;
                foreach (var piece in _allMapPieces)
                {
                    for (var x = 0; x < piece.Length; x++)
                    {
                        _serializedMap[index] = piece[x];
                        index++;
                    }
                }
                gameState.SerializedMap = _serializedMap;
                return true;
            }

            //Otherwise, Add the chunk of map to the list.
            else
            {
                _allMapPieces.Add(msg.SerializedMapChunk);
                return false;
            }
        }

        public Dictionary<int,HexTile> MakeServerMapFromArray()
        {
            var hexHolder = new GameObject();
            var result = new Dictionary<int,HexTile>();
            var hexMap = new HexMap(_serializedMap[0], _serializedMap[1]);
            HexTile.ParentMap = hexMap;
            for (var t = 2; t < _serializedMap.Length; t += 6)
            {
                //Serialized maps have large deserts of 0,0,0,0,0... on their tails. Cut this off when that happens.
                if (_serializedMap[t + 2] == 0) break;

                var tile = hexHolder.AddComponent<HexTile>();
                tile.X = _serializedMap[t];
                tile.Y = _serializedMap[t + 1];
                tile.Id = _serializedMap[t + 2];
                tile.Terrain = (Terrain) _serializedMap[t + 3];
                tile.Resource = new Resource(_serializedMap[t + 4], _serializedMap[t + 5]);

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

            return result;
        }

        public void MakeClientMapFromArray()
        {
            var mapObject = new GameObject {name = "Map"};
            var hexMap = new HexMap(_serializedMap[0], _serializedMap[1]);
            HexTile.ParentMap = hexMap;

            for (var t = 2; t < _serializedMap.Length; t += 6)
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
                tile.Terrain = (Terrain)_serializedMap[t+3];
                tile.Resource = new Resource(_serializedMap[t+4], _serializedMap[t+5]);
                hexObject.name = (tile.X + ", " + tile.Y);
                hexObject.transform.parent = mapObject.transform;

                //Add the tile to reference lists.
                hexMap.TileList.Add(tile);
            }
        }
    }
}
