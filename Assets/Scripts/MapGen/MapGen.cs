using System.Collections.Generic;
using Assets.Networking;
using UnityEngine;

namespace Assets.MapGen
{
    public class MapGen : MonoBehaviour {

        public int XZones, YZones;
        public GameObject HexPrefab;

        public void Launch (bool useSpiralPattern) {
            HexSector.HexPrefab = HexPrefab;
            HexTile.ParentMap = new HexMap(XZones*30,YZones*30);

            var zoneCoords = new List<Vector2>();
            if (useSpiralPattern)
            {
                zoneCoords.Add(new Vector2(1, 0));
                zoneCoords.Add(new Vector2(2, 0));
                zoneCoords.Add(new Vector2(0, 1));
                zoneCoords.Add(new Vector2(0, 2));
                zoneCoords.Add(new Vector2(1, 1));
                zoneCoords.Add(new Vector2(2, 1));
                zoneCoords.Add(new Vector2(1, 2));
            }
            else
            {
                for (int x = 0; x < XZones; x++)
                {
                    for (int y = 0; y < YZones; y++)
                    {
                        zoneCoords.Add(new Vector2(x, y));
                    }
                }
            }

            foreach (var coord in zoneCoords)
            {
                new HexRegion((int)coord.x, (int)coord.y);
            }

            for (int x = 0; x < HexTile.TileMap.GetLength(0); x++)
            {
                for (int y = 0; y < HexTile.TileMap.GetLength(1); y++)
                {
                    if (HexTile.TileMap[x, y] != null)
                    {
                        HexTile.TileMap[x,y].MapNeighbours();
                    }
                }
            }
            Debug.Log("Map Loaded!");
        }
    }
}
