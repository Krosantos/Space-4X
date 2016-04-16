using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class HexTile : MonoBehaviour, ISelectable
    {
        public static HexMap ParentMap;
        public List<HexTile>  Neighbours{
            get
            {
                var neighbours = new List<HexTile>();
                if (Ul != null) neighbours.Add(Ul);
                if (Uu != null) neighbours.Add(Uu);
                if (Ur != null) neighbours.Add(Ur);
                if (Dl != null) neighbours.Add(Dl);
                if (Dd != null) neighbours.Add(Dd);
                if (Dr != null) neighbours.Add(Dr);
                return neighbours;
            } 
        }
        public Unit OccupyUnit;

        public HexSector ParentSector;
        public Resource Resource;
        public Terrain Terrain;
        public GameObject Tint;
        public int X, Y, Id;

        public HexRegion ParentRegion
        {
            get { return ParentSector.ParentRegion; }
        }

        public static HexTile[,] TileMap
        {
            get { return ParentMap.AllTiles; }
            set { ParentMap.AllTiles = value; }
        }

        //Neighbours are properties: makes calculation a bit more CPU intensive, saves a ton of memory.
        public HexTile Ul
        {
            get
            {
                if (X%2 == 0 && X > 0)
                {
                    if (TileMap[X - 1, Y] != null) return TileMap[X - 1, Y];
                }
                else if (X > 0 && Y + 1 < TileMap.GetLength(1))
                {
                    if (TileMap[X - 1, Y + 1] != null) return TileMap[X - 1, Y + 1];
                }
                return null;
            }
        }

        public HexTile Uu
        {
            get
            {
                if (Y + 1 < TileMap.GetLength(1))
                {
                    if (TileMap[X, Y + 1] != null) return TileMap[X, Y + 1];
                }
                return null;
            }
        }

        public HexTile Ur
        {
            get
            {
                if (X%2 == 0 && X + 1 < TileMap.GetLength(0))
                {
                    if (TileMap[X + 1, Y] != null) return TileMap[X + 1, Y];
                }
                else if (X%2 != 0 && X + 1 < TileMap.GetLength(0) && Y + 1 < TileMap.GetLength(1))
                {
                    if (TileMap[X + 1, Y + 1] != null) return TileMap[X + 1, Y + 1];
                }
                return null;
            }
        }

        public HexTile Dl
        {
            get
            {
                if (X%2 == 0 && X > 0 && Y > 0)
                {
                    if (TileMap[X - 1, Y - 1] != null) return TileMap[X - 1, Y - 1];
                }
                else if (X%2 != 0 && X > 0)
                {
                    if (TileMap[X - 1, Y] != null) return TileMap[X - 1, Y];
                }
                return null;
            }
        }

        public HexTile Dd
        {
            get
            {
                if (Y > 0)
                {
                    if (TileMap[X, Y - 1] != null) return TileMap[X, Y - 1];
                }
                return null;
            }
        }

        public HexTile Dr
        {
            get
            {
                if (X%2 == 0 && X + 1 < TileMap.GetLength(0) && Y > 0)
                {
                    if (TileMap[X + 1, Y - 1] != null) return TileMap[X + 1, Y - 1];
                }
                else if (X%2 != 0 && X + 1 < TileMap.GetLength(0))
                {
                    if (TileMap[X + 1, Y] != null) return TileMap[X + 1, Y];
                }
                return null;
            }
        }

        public Color Color
        {
            get { return gameObject.GetComponent<SpriteRenderer>().color; }
            set { gameObject.GetComponent<SpriteRenderer>().color = value; }
        }

        public Color TintColor
        {
            get { return Tint.GetComponent<SpriteRenderer>().color; }
            set { Tint.GetComponent<SpriteRenderer>().color = value; }
        }

        public void OnSelect()
        {
            Debug.Log("I am a tile!");
            if (UiSelect.Type == SelectType.Unit)
            {
                Debug.Log("Previously, a unit was selected!");
            }
            UiSelect.Type = SelectType.Tile;
        }

        public void OnDeselect()
        {
        }

        private void Awake()
        {
            Resource = new Resource(0, 0);
        }

        public void OnMouseDown()
        {
            UiSelect.Select(this);
        }
    }

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
            var result = new int[2 + TileList.Count*6];
            result[0] = _xDim;
            result[1] = _yDim;
            var index = 2;
            foreach (var tile in TileList)
            {
                result[index] = tile.X;
                result[index + 1] = tile.Y;
                result[index + 2] = tile.Id;
                result[index + 3] = (int) tile.Terrain;
                result[index + 4] = (int) tile.Resource.Type;
                result[index + 5] = tile.Resource.Quantity;
                index += 6;
            }
            return result;
        }
    }
}