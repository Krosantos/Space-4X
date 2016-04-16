using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class HexSector {

        public bool IsEven;
        public int X, Y;
        public List<HexTile> ChildTiles;
        public HexRegion ParentRegion;
        public static GameObject HexPrefab;

        public List<Vector2> GetTileMapping
        {
            get
            {
                var result = new List<Vector2>
                {
                    new Vector2(0, 3),
                    new Vector2(0, 4),
                    new Vector2(1, 2),
                    new Vector2(1, 3),
                    new Vector2(1, 4),
                    new Vector2(1, 5),
                    new Vector2(1, 6),
                    new Vector2(2, 1),
                    new Vector2(2, 2),
                    new Vector2(2, 3),
                    new Vector2(2, 4),
                    new Vector2(2, 5),
                    new Vector2(2, 6),
                    new Vector2(2, 7),
                    new Vector2(3, 0),
                    new Vector2(3, 1),
                    new Vector2(3, 2),
                    new Vector2(3, 3),
                    new Vector2(3, 4),
                    new Vector2(3, 5),
                    new Vector2(3, 6),
                    new Vector2(4, 1),
                    new Vector2(4, 2),
                    new Vector2(4, 3),
                    new Vector2(4, 4),
                    new Vector2(4, 5),
                    new Vector2(4, 6),
                    new Vector2(4, 7),
                    new Vector2(5, 1),
                    new Vector2(5, 2),
                    new Vector2(5, 3),
                    new Vector2(5, 4),
                    new Vector2(5, 5),
                    new Vector2(5, 6),
                    new Vector2(5, 7),
                    new Vector2(6, 1),
                    new Vector2(6, 2),
                    new Vector2(6, 3),
                    new Vector2(6, 4),
                    new Vector2(6, 5),
                    new Vector2(6, 6),
                    new Vector2(6, 7),
                    new Vector2(7, 1),
                    new Vector2(7, 2),
                    new Vector2(7, 3),
                    new Vector2(7, 4),
                    new Vector2(7, 5),
                    new Vector2(8, 4),
                    new Vector2(8, 5)
                };
                return result;
            }
        }

        public HexSector(int x, int y, HexRegion parentRegion)
        {
            ParentRegion = parentRegion;
            X = x;
            Y = y;
            IsEven = (X % 2 == 0);
            ChildTiles = new List<HexTile>();

            foreach(var coord in GetTileMapping)
            {            
                var tileXCord = X*7 + (int)coord.x;
                var tileYCord = Y*7 + (int)coord.y;

                var tileXPos = X * 5.7f + coord.x * 0.815f;
                var tileYPos = Y * 7f + coord.y;

                if (IsEven) {
                    tileYPos += 3.5f;
                    tileYCord += 3;
                }
                else if (coord.x%2 == 0)
                {
                    tileYCord --;
                }

                GameObject hexObject;

                if (coord.x % 2 == 0)
                {
                    hexObject = Object.Instantiate(HexPrefab, new Vector3(tileXPos, tileYPos, 0), Quaternion.identity) as GameObject;
                }
                else
                {
                    hexObject = Object.Instantiate(HexPrefab, new Vector3(tileXPos, tileYPos + 0.5f, 0), Quaternion.identity) as GameObject;
                }
                var tile = hexObject.GetComponent<HexTile>();


                HexTile.ParentMap.TileList.Add(tile);
            
                ChildTiles.Add(tile);
                tile.ParentSector = this;
                HexTile.TileMap[tileXCord, tileYCord] = tile;
                HexTile.ParentMap.TileList.Add(tile);
                tile.X = tileXCord;
                tile.Y = tileYCord;
                hexObject.name = (tile.X+", "+tile.Y);
                hexObject.transform.parent = MapGen.Map.transform;
                //tile.Id = MapGen.IdGen.GenerateId();
            }
        }
    }
}
