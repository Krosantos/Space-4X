using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class MapGen {

        public int XZones, YZones;
        public static GameObject HexPrefab
        {
            get { return Resources.Load<GameObject>("PlaceholderHex"); }
        }
        public static GameObject Map;

        public void Launch (bool useSpiralPattern) {
            Map = new GameObject {name = "Map"};
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

                /*
                zoneCoords.Add(new Vector2(3, 1));
                zoneCoords.Add(new Vector2(4, 1));
                zoneCoords.Add(new Vector2(2, 2));
                zoneCoords.Add(new Vector2(2, 3));
                zoneCoords.Add(new Vector2(3, 2));
                zoneCoords.Add(new Vector2(4, 2));
                zoneCoords.Add(new Vector2(3, 3));
                
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

            foreach (var coord in zoneCoords)
            {
                new HexRegion((int)coord.x, (int)coord.y);
            }
        }
    }
}
