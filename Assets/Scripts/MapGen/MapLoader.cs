using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class MapLoader : MonoBehaviour
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
                        _serializedMap[index] = x;
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
            var result = new Dictionary<int,HexTile>();

            for (var t = 2; t < _serializedMap.Length; t += 6)
            {
                var tile = new HexTile
                {
                    X = _serializedMap[t],
                    Y = _serializedMap[t + 1],
                    Id = _serializedMap[t + 2],
                    Terrain = (Terrain)_serializedMap[t + 3],
                    Resource = new Resource(_serializedMap[t + 4], _serializedMap[t + 5])
                };

                //Add the tile to reference lists.
                result.Add(tile.Id,tile);
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
                GameObject hexObject;
                if (_serializedMap[t] % 2 == 0)
                {
                    hexObject = Instantiate(HexPrefab, new Vector3(_serializedMap[t] * 0.815f, _serializedMap[t+1], 0), Quaternion.identity) as GameObject;
                }
                else
                {
                    hexObject = Instantiate(HexPrefab, new Vector3(_serializedMap[t] * 0.815f, _serializedMap[t+1] + 0.5f, 0), Quaternion.identity) as GameObject;
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
