using System.Collections.Generic;

namespace Assets.Scripts.MapGen
{
    public class HexMap
    {
        public HexTile[,] AllTiles;
        public List<HexTile> TileList;
        private readonly int _xDim;
        private readonly int _yDim;

        public HexMap()
        {
            TileList = new List<HexTile>();
        }

        public HexMap(int x, int y)
        {
            _xDim = x;
            _yDim = y;
            AllTiles = new HexTile[x, y];
            TileList = new List<HexTile>();
        }

        public int[] SerializeMap()
        {
            var result = new int[2 + TileList.Count * 6];
            result[0] = _xDim;
            result[1] = _yDim;
            var index = 2;
            foreach (var tile in TileList)
            {
                result[index] = tile.X;
                result[index + 1] = tile.Y;
                result[index + 2] = tile.Id;
                result[index + 3] = (int)tile.Terrain;
                result[index + 4] = (int)tile.Resource.Type;
                result[index + 5] = tile.Resource.Quantity;
                index += 6;
            }
            return result;
        }
    }
}
