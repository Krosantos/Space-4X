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
        public int MixedScore, AsteroidScore, IonScore, DeadspaceScore;
    }
    public class MapGen {

        public int XZones, YZones;
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
        public static GameObject Map;

        public void Launch (MapSetting setting) {
            Map = new GameObject {name = "Map"};

            var zoneCoords = new List<Vector2>();
            if (false)
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
                for (var x = 0; x < XZones; x++)
                {
                    for (var y = 0; y < YZones; y++)
                    {
                        zoneCoords.Add(new Vector2(x, y));
                    }
                }
            }
            //Normalize the coordinates to be based off 0.
            var xMin = zoneCoords.Select(r => r.x).Min();
            var yMin = zoneCoords.Select(r => r.y).Min();
            var xRange = zoneCoords.Select(r => r.x).Max() - xMin;
            var yRange = zoneCoords.Select(r => r.y).Max() - yMin;

            HexTile.ParentMap = new HexMap(XZones*20 + YZones*8, YZones*18 + XZones*6);

            //Debug.Log("Dimensions: "+HexTile.ParentMap.AllTiles.GetLength(0)+", " + HexTile.ParentMap.AllTiles.GetLength(1));

            var regionList = zoneCoords.Select(coord => new HexRegion((int) (coord.x - xMin), (int) (coord.y - yMin))).ToList();

            var list = HexTile.ParentMap.TileList;
            var highestX = list.Select(t => t.X).Max();
            var highestY = list.Select(t => t.Y).Max();
            var lowestX = list.Select(t => t.X).Min();
            var lowestY = list.Select(t => t.Y).Min();

            Debug.Log("Highest X: " + highestX + " Highest Y: " + highestY);
            Debug.Log("RegionX: " + regionList.Select(x => x.X).Max() + " RegionY: " + regionList.Select(y => y.Y).Max());
            regionList.AssignAllRegions(setting);

        }
    }
}
