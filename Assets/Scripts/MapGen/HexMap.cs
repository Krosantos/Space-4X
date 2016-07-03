using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class HexMap
    {
        public HexTile[,] AllTiles;
        public List<HexTile> TileList;

        public HexMap()
        {
            TileList = new List<HexTile>();
        }

        public HexMap(int x, int y)
        {
            AllTiles = new HexTile[x, y];
            TileList = new List<HexTile>();
        }

        public int[] SerializeMap()
        {
            var result = new int[2 + TileList.Count * 8];
            result[0] = TileList.Max(x => x.X) + 1;
            result[1] = TileList.Max(y => y.Y) + 1;
            Debug.Log("The serialized map we're sending is " + result[0] + " by " + result[1] + " tiles large.");
            var index = 2;
            foreach (var tile in TileList)
            {
                result[index] = tile.X;
                result[index + 1] = tile.Y;
                result[index + 2] = tile.Id;
                result[index + 3] = (int)tile.Terrain;
                result[index + 4] = (int)tile.Resource.Type;
                result[index + 5] = tile.Resource.Quantity;
                result[index + 6] = tile.Resource.RegenQuantity;
                result[index + 7] = tile.Resource.TurnsToRegen;
                index += 8;
            }
            return result;
        }
    }
}
