using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapLoader : MonoBehaviour
{
    public static GameObject HexPrefab
    {
        get { return Resources.Load<GameObject>("PlaceholderHex"); }
    }

    public static Client Player;
    public static List<int[]> AllMapPieces;
    public static int[] SerializedMap;

    public static void AddPacketToMap(LoadMapMsg msg)
    {
        //Initialize the list if not present.
        if(AllMapPieces == null) AllMapPieces = new List<int[]>();

        //If it's the final chunk, create the serialized map.
        if (msg.IsFinalPiece)
        {
            Debug.Log("We have the entire map!");
            SerializedMap = new int[AllMapPieces.Sum(x => x.Length)];
            var index = 0;
            foreach (var piece in AllMapPieces)
            {
                for (int x = 0; x < piece.Length; x++)
                {
                    SerializedMap[index] = piece[x];
                    index++;
                }
            }
            LoadMapFromSerializedArray(SerializedMap, Player);
        }

        //Otherwise, Add the chunk of map to the list.
        else AllMapPieces.Add(msg.SerializedMapChunk);
    }

    public static void LoadMapFromSerializedArray(int[] serializedMap, Client player)
    {
        var hexMap = new HexMap(serializedMap[0],serializedMap[1]);
        HexTile.ParentMap = hexMap;

        for (int t = 2; t < serializedMap.Length; t += 6)
        {
            GameObject hexObject;
            if (serializedMap[t] % 2 == 0)
            {
                hexObject = Instantiate(HexPrefab, new Vector3(serializedMap[t] * 0.815f, serializedMap[t+1], 0), Quaternion.identity) as GameObject;
            }
            else
            {
                hexObject = Instantiate(HexPrefab, new Vector3(serializedMap[t] * 0.815f, serializedMap[t+1] + 0.5f, 0), Quaternion.identity) as GameObject;
            }
            var tile = hexObject.GetComponent<HexTile>();
            hexMap.allTiles[serializedMap[t], serializedMap[t+1]] = tile;
            tile.x = serializedMap[t];
            tile.y = serializedMap[t+1];
            tile.Id = serializedMap[t+2];
            tile.Terrain = (Terrain)serializedMap[t+3];
            tile.Resource = new Resource(serializedMap[t+4], serializedMap[t+5]);

            //Add the tile to reference lists.
            hexMap.tileList.Add(tile);
            player.AllTiles.Add(tile.Id, tile);
        }
        //Now that the individual tiles are spawned, have them map neighbours and proceed.
        foreach (var tile in hexMap.tileList)
        {
            tile.MapNeighbours();

            //Later, change sprite/color to denote terrain.
        }
    }
}
