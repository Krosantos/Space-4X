using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using Terrain = Assets.Scripts.Utility.Terrain;

namespace Assets.Scripts.MapGen
{
    public class HexTile : MonoBehaviour, ISelectable
    {
        private static int _idGen;
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

        public Sprite Sprite
        {
            get { return gameObject.GetComponent<SpriteRenderer>().sprite; }
            set { gameObject.GetComponent<SpriteRenderer>().sprite = value; }
        }

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
            Debug.Log("Select Tile!");
            if (OccupyUnit == null)
            {
                UiSelect.Type = SelectType.Tile;
            }
            else
            {
                OccupyUnit.SelectSelf();
            }
        }

        public void OnDeselect()
        {
        }

        [UsedImplicitly]
        private void Awake()
        {
            Resource = new Resource(0, 0, 0, 0);
        }

        public void SetId()
        {
            _idGen++;
            Id = _idGen;
        }

        public void SetTerrain(Terrain type)
        {
            Terrain = type;
            Sprite = Resources.Load<Sprite>("Sprites/Map/" + Terrain);
        }

        public void OnMouseDown()
        {
            this.SelectSelf();
        }
    }
}