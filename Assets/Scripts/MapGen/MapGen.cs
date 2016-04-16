using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGen
{
    public class MapGen : MonoBehaviour {

        public int XZones, YZones;
        public GameObject HexPrefab;
        public static GameObject Map;
        public void Launch (bool useSpiralPattern) {
            Map = new GameObject {name = "Map"};
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
            Debug.Log("Map Loaded!");
        }
    }
}
