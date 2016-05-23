using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class MapSetting
    {
        public bool Spiral;
        public int PlayerCount;
        public int RichnessTotal;
        public int XZones, YZones;
        public int MixedScore, AsteroidScore, IonScore, DeadspaceScore;
    }
    public class MapGen {
        
        public List<HexRegion> RegionList; 
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
        public static GameObject Map;

        public void SpawnTiles (MapSetting setting) {
            Map = new GameObject {name = "Map"};
            HexTile.ParentMap = new HexMap();
            var zoneCoords = new List<Vector2>();
            if (setting.Spiral)
            {
                
                zoneCoords.Add(new Vector2(1, 0));
                zoneCoords.Add(new Vector2(2, 0));
                zoneCoords.Add(new Vector2(0, 1));
                zoneCoords.Add(new Vector2(0, 2));
                zoneCoords.Add(new Vector2(1, 1));
                zoneCoords.Add(new Vector2(2, 1));
                zoneCoords.Add(new Vector2(1, 2));

                
                zoneCoords.Add(new Vector2(3, 1));
                zoneCoords.Add(new Vector2(4, 1));
                zoneCoords.Add(new Vector2(2, 2));
                zoneCoords.Add(new Vector2(2, 3));
                zoneCoords.Add(new Vector2(3, 2));
                zoneCoords.Add(new Vector2(4, 2));
                zoneCoords.Add(new Vector2(3, 3));
                /*
                zoneCoords.Add(new Vector2(0, 3));
                zoneCoords.Add(new Vector2(1, 3));
                zoneCoords.Add(new Vector2(-1, 4));
                zoneCoords.Add(new Vector2(-1, 5));
                zoneCoords.Add(new Vector2(0, 4));
                zoneCoords.Add(new Vector2(1, 4));
                zoneCoords.Add(new Vector2(0, 5));
                */
            }
            else
            {
                for (var x = 0; x < setting.XZones; x++)
                {
                    for (var y = 0; y < setting.YZones; y++)
                    {
                        zoneCoords.Add(new Vector2(x, y));
                    }
                }
            }
            //Normalize the coordinates to be based off 0.
            var xMin = zoneCoords.Select(r => r.x).Min();
            var yMin = zoneCoords.Select(r => r.y).Min();

            //Actually spawn all our tiles (regions spawn sectors, sectors spawn tiles).
            RegionList = zoneCoords.Select(coord => new HexRegion((int) (coord.x - xMin), (int) (coord.y - yMin))).ToList();
        }

        public void MapTiles()
        {
            var list = HexTile.ParentMap.TileList;
            var xMin = list.Select(t => t.X).Min();
            var yMin = list.Select(t => t.Y).Min();
            var xMax = list.Select(t => t.X).Max();
            var yMax = list.Select(t => t.Y).Max();

            HexTile.ParentMap.AllTiles = new HexTile[xMax - xMin + 1, yMax - yMin + 1];
            //Set tiles to a 0 index, and stick them in the map.
            foreach (var tile in list)
            {
                tile.X -= xMin;
                tile.Y -= yMin;
                HexTile.ParentMap.AllTiles[tile.X, tile.Y] = tile;
            }
        }

        public void AssignTypes(MapSetting setting)
        {
            RegionList.AssignAllRegions(setting);
        }
    }
}
